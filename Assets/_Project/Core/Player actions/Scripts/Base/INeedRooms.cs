using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Maps;
using Core.Selection.Rooms;
using UnityEngine;

namespace Core.PlayerActions
{
    public interface INeedRooms
    {
        public int RequredRoomsCount { get; }
        public RoomCell[] RoomSelection { get; set; }
        public RoomCell[] RoomSelectionSource { get; }
    }
}
