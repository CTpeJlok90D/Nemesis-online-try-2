using UnityEngine;

namespace Core.Maps
{
    public interface IRoomOwnerChangedHandler
    {
        public void OnRoomMove(RoomCell oldRoom, RoomCell newRoom);
    }
}
