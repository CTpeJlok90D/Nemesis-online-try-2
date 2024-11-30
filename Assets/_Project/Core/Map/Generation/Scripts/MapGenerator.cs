using System.Collections.Generic;
using System.Linq;
using Core.DestinationCoordinats;
using Core.Engines;
using Core.EscapePods;
using Core.Map.IntellegenceTokens;
using Core.PlayerTablets;
using Core.TabletopRandom;
using UnityEngine;
using Zenject;

namespace Core.Maps.Generation
{
    public class MapGenerator : MonoBehaviour
    {
        [Inject] private Map _map;

        [Inject] private MapGeneratorConfiguration _mapGeneratorConfiguration;

        [Inject] private PlayerTabletList _playerTabletList;

        private Dictionary<int, Bag<RoomContent>> _runtimeBags;
        
        public void Generate()
        {
            GenerateDestinationCards();
            GenerateRooms();
            GenerateTokens();
            GenerateEscapePods();
            GenerateShipEngines();
        }

        private void GenerateShipEngines()
        {
            foreach (ShipEngine shipEngine in _map.ShipEngies)
            {
                shipEngine.IsWorking.Value = Random.Range(0,2) == 0;
            }
        }

        private void GenerateEscapePods()
        {
            int enablePodsCount;
            if (_playerTabletList.ActiveTablets.Length >= _mapGeneratorConfiguration.EscapePodCountPerPlayer.Length)
            {
                enablePodsCount = _mapGeneratorConfiguration.EscapePodCountPerPlayer.Last();
                Debug.LogWarning($"[<color=yellow>Generation warning</color>] map config do not cotains capsule count for {_mapGeneratorConfiguration.EscapePodCountPerPlayer.Length} players count. Using max");
            }
            else
            {
                enablePodsCount = _mapGeneratorConfiguration.EscapePodCountPerPlayer[_playerTabletList.ActiveTablets.Length];
            }

            foreach (EscapePod escapePod in _map.EscapePods)
            {
                if (enablePodsCount <= 0)
                {
                    _map.RemoveEscapePod(escapePod);
                }
                else
                {
                    enablePodsCount--;
                }
            }
        }

        private void GenerateRooms()
        {
            if (_mapGeneratorConfiguration.BagsOfRooms.Count == 0)
            {
                throw new MapGenerationException("Map generation configuration have no rooms"); 
            }

            _runtimeBags = new(_mapGeneratorConfiguration.BagsOfRooms);

            foreach (int key in _runtimeBags.Keys.ToArray())
            {
                _runtimeBags[key] = _runtimeBags[key].Clone();
            }

            bool roomsIsOut = false;

            foreach (RoomCell roomCell in _map)
            {
                if (_runtimeBags.TryGetValue(roomCell.Layer, out Bag<RoomContent> bagOfRooms) == false)
                {
                    continue;   
                }

                RoomContent content = bagOfRooms.PickOne();
                roomCell.Init(content);

                if (bagOfRooms.Items.Count == 0)
                {
                    _runtimeBags[roomCell.Layer] = new(_mapGeneratorConfiguration.BagsOfRooms[roomCell.Layer].Items);
                    roomsIsOut = true;
                }
            }

            if (roomsIsOut)
            {
                Debug.LogWarning("[<color=yellow>Generation warning</color>] The rooms are over. There are too few rooms when generating. Duplication is possible.");
            }
        }

        private void GenerateTokens()
        {
            bool tokensIsOut = false;
            Bag<IntelegenceToken> _tokens = new(_mapGeneratorConfiguration.IntelegenceTokens);

            if (_mapGeneratorConfiguration.IntelegenceTokens.Length == 0)
            {
                throw new MapGenerationException("Map generation configuration have no intekegence tokens"); 
            }

            foreach (RoomCell roomCell in _map.Where(x => x.GenerateIntellegenceToken))
            {
                if (_tokens.Items.Count == 0)
                {
                    _tokens = new(_mapGeneratorConfiguration.IntelegenceTokens);
                    tokensIsOut = true;
                }
                roomCell.IntellegenceTokenNet.Value = _tokens.PickOne();
            }

            if (tokensIsOut)
            {
                Debug.LogWarning("[<color=yellow>Generation warning</color>] The intellegence token are over. There are too few tokens when generating. Duplication is possible.");
            }
        }

        private void GenerateDestinationCards()
        {
            DestinationCoordinatsCard[] availableCards = _mapGeneratorConfiguration.AvailableDestinationCards.ToArray();
            
            if (availableCards.Length == 0)
            {
                throw new MapGenerationException("Destination cards in map generation config not found");
            }

            DestinationCoordinatsCard card = availableCards[Random.Range(0, availableCards.Length)];
            _map.DestinationCoordinatsCard.Value = card;
        }
    }
}
