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
        [Inject] private Ship _ship;
        private void Awake()
        {
            DebugLogConsole.AddCommand<int>("Map.Summon", "Summons a creature in a specified room with a specified number", SummonEnemy);
        }

        private void SummonEnemy(int roomIndex)
        {
            try
            {
                RoomCell cell = _ship.RoomCells.ElementAt(roomIndex - 1);
                _ = _ship.SummonEnemyIn(cell);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
