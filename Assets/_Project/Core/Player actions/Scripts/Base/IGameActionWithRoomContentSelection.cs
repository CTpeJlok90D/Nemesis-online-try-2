using Core.Maps;

namespace Core.PlayerActions.Base
{
    public interface IGameActionWithRoomContentSelection
    {
        public RoomContent[] RoomContentSelection { get; set; }
        
        public RoomContent[] RoomContentSelectionSource { get; }
        
        public int RequiredRoomContentCount { get; }
    }
}
