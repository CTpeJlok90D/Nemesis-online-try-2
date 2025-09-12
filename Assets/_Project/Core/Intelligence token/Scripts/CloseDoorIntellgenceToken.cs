using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using Core.Maps.IntellegenceTokens;
using Core.PlayerTablets;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token action/Close door intellegence token")]
    public class CloseDoorIntellgenceToken : IntelegenceTokenAction
    {
        public override void Execute(RoomCell selfRoom, RoomCell roomExecutorCameFrom, PlayerTablet executor)
        {
            List<Tunnel> tunnels = selfRoom.Tunnels.OfType<Tunnel>().Where(x => x.RoomCells.Contains(roomExecutorCameFrom)).ToList();
            Tunnel tunnel = tunnels.First(x => x.DoorState is not DoorState.Closed);
            tunnel.CloseDoor();
        }
    }
}