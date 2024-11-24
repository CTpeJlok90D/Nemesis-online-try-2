using System.Collections.Generic;
using System.Linq;
using Core.TabletopRandom;
using UnityEngine;
using Zenject;

namespace Core.Maps.Generation
{
    public class MapGenerator : MonoBehaviour
    {
        [Inject] private Map _map;
        [Inject] private MapGeneratorConfiguration _mapGeneratorConfiguration;

        private Dictionary<int, Bag<RoomContent>> _runtimeBags;
        
        public void Generate()
        {
            _runtimeBags = new(_mapGeneratorConfiguration.BagsOfRooms);
            foreach (int key in _runtimeBags.Keys.ToArray())
            {
                _runtimeBags[key] = _runtimeBags[key].Clone();
            }

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
                    Debug.LogWarning("The rooms are over. There are too few rooms when generating. Duplication is possible.");
                }
            }
        }
    }
}
