using System;
using System.Linq;
using Core.PlayerActions;
using Core.TimeTracks;
using UnityEngine;

namespace Core.Maps
{
    [CreateAssetMenu(menuName = "Game/Maps/Actions/Hibernation compartment action", fileName = "Hibernation compartment action")]
    public class HibernationCompartmentAction : RoomAction
    {
        private const int TimeTrackValueToActiveAction = 8;

        public override IGameAction.CanExecuteCheckResult CanExecute()
        {
            TimeTrack timeTrack = TimeTrack.Instances.First(x => x.Type is TimeTrackType.Main);

            IGameAction.CanExecuteCheckResult result = new()
            {
                Result = timeTrack.Current.Value <= TimeTrackValueToActiveAction
            };

            if (result.Result == false)
            {
                result.Error = new InvalidOperationException("It's too early to board a hibernation capsule.");
            }

            return result;
        }

        public override void ForceExecute()
        {
            Executor.EnterHybridizationCapsule();
        }
    }
}