using System;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerTablets;
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

        public override void Execute(PlayerTablet execoter, IPayment[] payments, GameObject[] targets)
        {
            Debug.Log($"{this} was executed");
        }
    }
}
