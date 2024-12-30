using UnityEngine;

namespace Core.Maps
{
    public class RoomOwnerPosition : MonoBehaviour, IRoomOwnerChangedHandler
    {
        private float _randomPositionRadius = 4f;
        public void OnRoomMove(RoomCell oldRoom, RoomCell newRoom)
        {
            Vector3 newPosition = newRoom.transform.position * Random.insideUnitCircle * _randomPositionRadius;
            transform.position = newPosition;
        }
    }
}
