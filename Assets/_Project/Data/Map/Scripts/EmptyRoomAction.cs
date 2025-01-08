using System;
using Core.Maps;
using UnityEngine;

namespace Data.Maps
{
    [CreateAssetMenu(menuName = "Game/Maps/Actions/Empty")]
    public class EmptyRoomAction : RoomAction, IEquatable<EmptyRoomAction>
    {
        public bool Equals(EmptyRoomAction other)
        {
            return true;
        }
    }
}
