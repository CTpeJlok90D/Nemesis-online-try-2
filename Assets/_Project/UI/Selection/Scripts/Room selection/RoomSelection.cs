using Core.Maps;
using Core.PlayerActions;
using UI.SelectionBase;

namespace UI.Selection.Rooms
{
    public class RoomSelection : Selection<RoomCell>
    {
        public RoomSelection(int maxCount) : base(maxCount)
        {
            
        }

        public override void Add(RoomCell value)
        {
            base.Add(value);
            PlayerActionExecutor.Instance.AddRoomToSelection(value);
        }

        public override void Remove(RoomCell value)
        {
            base.Remove(value);
            PlayerActionExecutor.Instance.RemoveRoomFromSelection(value);
        }
    }
}
