using UnityEngine;

namespace Core.Maps
{
    public class RoomOwnerPosition : MonoBehaviour, IRoomOwnerChangedHandler
    {
        public void OnRoomMove(RoomCell oldRoom, RoomCell newRoom)
        {
            transform.position = newRoom.transform.position;
        }
    }
}
