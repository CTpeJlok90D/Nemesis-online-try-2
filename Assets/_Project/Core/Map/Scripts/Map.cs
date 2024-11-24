using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-map-96.png")]
    public class Map : MonoBehaviour, IEnumerable<RoomCell>
    {
        [SerializeField] private RoomCell[] _roomCells;

        public IReadOnlyCollection<RoomCell> RoomCells => _roomCells;

        public IEnumerator<RoomCell> GetEnumerator()
        {
            foreach (RoomCell cell in _roomCells)
            {
                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _roomCells.GetEnumerator();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _roomCells = GetComponentsInChildren<RoomCell>();
        }
#endif
    }
}
