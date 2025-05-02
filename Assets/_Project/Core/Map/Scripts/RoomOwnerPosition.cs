using UnityEngine;

namespace Core.Maps
{
    public class RoomOwnerPosition : MonoBehaviour, IRoomOwnerChangedHandler
    {
        [SerializeField] private float _randomPositionRadius = 2.5f;
        public void OnRoomMove(RoomCell oldRoom, RoomCell newRoom)
        {
            Vector3 newPosition = newRoom.transform.position + new Vector3(Random.Range(-_randomPositionRadius, _randomPositionRadius),0, Random.Range(-_randomPositionRadius, _randomPositionRadius));
            transform.position = newPosition;
        }
    }
}
