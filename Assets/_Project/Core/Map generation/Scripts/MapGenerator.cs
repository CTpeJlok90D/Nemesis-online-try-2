using System.Collections.Generic;
using System.Linq;
using Core.AliensTablets;
using Core.DestinationCoordinats;
using Core.Engines;
using Core.EscapePods;
using Core.Maps.IntellegenceTokens;
using Core.PlayerTablets;
using Core.TabletopRandom;
using Core.AliensBags;
using UnityEngine;
using Zenject;
using Core.Aliens;

namespace Core.Maps.Generation
{
    public class MapGenerator : MonoBehaviour
    {
        [Inject] private Map _map;

        [Inject] private MapGeneratorConfiguration _mapGeneratorConfiguration;

        [Inject] private PlayerTabletList _playerTabletList;

        [Inject] private AliensTablet _aliensTablet;

        [Inject] private AliensBag _aliensBag;

        private Dictionary<int, Bag<RoomType>> _runtimeBags;

        public delegate void GeneratedHandle(MapGenerator sender);
        public event GeneratedHandle Generated;
        
        public void Generate()
        {
            GenerateDestinationCards();
            GenerateRooms();
            GenerateIntelegenceTokens();
            GenerateEscapePods();
            GenerateShipEngines();
            GenerateAlienTablets();
            GenerateAliensBug();
            Generated?.Invoke(this);
        }

        private void GenerateAliensBug()
        {
            if (_mapGeneratorConfiguration.DefaultAliensBag.Count() == 0)
            {
                Debug.LogWarning($"[<color=yellow>Generation warning</color>] Default aliens bag is empty!");
            }

            if (_mapGeneratorConfiguration.AddictionalAliensPerPlayer.Count() == 0)
            {
                Debug.LogWarning($"[<color=yellow>Generation warning</color>] Addictional aliens bag is empty!");
            }

            List<AlienToken> result = new();

            result.AddRange(_mapGeneratorConfiguration.DefaultAliensBag);

            for (int i = 0; i < _playerTabletList.Count(); i++)
            {
                result.AddRange(_mapGeneratorConfiguration.AddictionalAliensPerPlayer);
            }

            _aliensBag.Initialize(result);
        }

        private void GenerateAlienTablets()
        {
            List<AlienWeaknessCard> result = new List<AlienWeaknessCard>();
            Bag<AlienWeaknessCard> deckCopy = new(_mapGeneratorConfiguration.AlienWeaknessCards);
            while (result.Count < MapGeneratorConfiguration.WEAKNESS_CARDS_COUNT)
            {
                result.Add(deckCopy.PickOne());
            }
            _aliensTablet.Initialize(result.ToArray());
        }

        private void GenerateShipEngines()
        {
            foreach (ShipEngine shipEngine in _map.ShipEngines)
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

            foreach (EscapePod escapePod in _map.EscapePods.ToArray())
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
                if (_runtimeBags.ContainsKey(roomCell.Layer) == false)
                {
                    continue;   
                }

                RoomType content = _runtimeBags[roomCell.Layer].PickOne();
                roomCell.Init(content);

                if (_runtimeBags[roomCell.Layer].Items.Count == 0)
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

        private void GenerateIntelegenceTokens()
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
