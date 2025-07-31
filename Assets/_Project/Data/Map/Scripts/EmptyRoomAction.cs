using System;
using Core.Maps;
using Core.PlayerActions;
using UnityEngine;

namespace Data.Maps
{
    [CreateAssetMenu(menuName = "Game/Maps/Actions/Empty")]
    public class EmptyRoomAction : RoomAction, IEquatable<EmptyRoomAction>
    {
        public override int RequaredPaymentCount => 0;

        public bool Equals(EmptyRoomAction other)
        {
            return other == this;
        }

        public override IGameAction.CanExecuteCheckResult CanExecute()
        {
            return new IGameAction.CanExecuteCheckResult()
            {
                Result = true,
            };
        }

        public override void ForceExecute()
        {
            Debug.Log($"{Executor.Nickname} is executing empty action at {Room}", Room);
        }
    }
}
