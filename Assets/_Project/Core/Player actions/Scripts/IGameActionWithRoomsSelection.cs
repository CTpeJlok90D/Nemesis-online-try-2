using System.Linq;
using Core.Maps;
using Core.Selection.Rooms;

namespace Core.PlayerActions
{
    public interface IGameActionWithRoomsSelection
    {
        public int RequredRoomsCount { get; }
        public RoomCell[] Selection { get; set; }
    }

    public static class IGameActionWithRoomsSelectionExcentions
    {
        public static void Initialize(this IGameActionWithRoomsSelection gameActionWithSelection, RoomSelection roomSelection)
        {
            gameActionWithSelection.Selection = roomSelection.ToArray();
        }
    }
}
