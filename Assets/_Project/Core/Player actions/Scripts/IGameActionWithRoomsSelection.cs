using System.Linq;
using System.Threading.Tasks;
using Core.Maps;

namespace Core.PlayerActions
{
    public interface IGameActionWithRoomsSelection
    {
        public int RequredRoomsCount { get; }
        public bool CanAddRoomToSelection(RoomCell roomCell);
    }

    public static class AdderToRoomCellSelection
    {
        public static void AddToSelection(this IGameActionWithRoomsSelection gameActionWithSelection, RoomCell objectToAdd)
        {
            if (gameActionWithSelection.CanAddRoomToSelection(objectToAdd))
            {
                PlayerActionExecutor.Instance.AddRoomToSelection(objectToAdd);
            }
        }

        public static bool RemoveFromSelection(this IGameActionWithPayment gameActionWithSelection, RoomCell objectToRemove)
        {
            return PlayerActionExecutor.Instance.RemoveRoomFromSelection(objectToRemove);
        }

        public static int GetSelectedCount(this IGameActionWithPayment gameActionWithSelection)
        {
            return PlayerActionExecutor.Instance.RoomSelection.Length;
        }

        public static RoomCell[] GetSelectedRooms(this IGameActionWithPayment gameActionWithSelection)
        {
            return PlayerActionExecutor.Instance.RoomSelection;
        }
    }
}
