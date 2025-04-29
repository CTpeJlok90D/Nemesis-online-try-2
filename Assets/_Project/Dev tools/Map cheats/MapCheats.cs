using System;
using System.Linq;
using Core.Maps;
using IngameDebugConsole;
using UnityEngine;
using Zenject;

namespace Devtools.Maps
{
    public class MapCheats : MonoBehaviour
    {
        [Inject] private Map _map;
        private void Awake()
        {
            DebugLogConsole.AddCommand<int>("Map.Summon", "Summons a creature in a specified room with a specified number", SummonEnemy);
        }

        private void SummonEnemy(int roomIndex)
        {
            try
            {
                RoomCell cell = _map.RoomCells.ElementAt(roomIndex - 1);
                _map.SummonEnemyIn(cell);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
