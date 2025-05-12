using Core.Maps;

namespace Core.PlayerActions.Base
{
    public interface INeedRoomContents
    {
        public RoomContent[] RoomContentSelection { get; set; }
        
        public RoomContent[] RoomContentSelectionSource { get; }
        
        public int RequiredRoomContentCount { get; }
    }
}
