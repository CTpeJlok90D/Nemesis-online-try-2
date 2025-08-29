using System.Linq;
using Core.DestinationCoordinats;
using Core.Maps;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.Missions
{
    [Icon("Assets/_Project/Core/Missions/Editor/icons8-mission-96.png")]
    public abstract class MissionTarget : ScriptableObject
    {
        private const PlayerTag SignalToken = PlayerTag.Signal;
        [field: SerializeField] public Destination TargetDestination { get; private set; }
        [field: SerializeField] public bool NeedSignal { get; private set; }
        protected abstract bool IsConditionsAreMet(PlayerTablet executor);

        public virtual bool IsSurvived(PlayerTablet executor)
        {
            return executor.IsDead == false && 
                   (Ship.Instance.IsDestroyed == false && Ship.Instance.Destination == TargetDestination
                    || executor.InSaveCapsule);
        }
        
        public bool IsCompletedFor(PlayerTablet executor)
        {
            return IsConditionsAreMet(executor) && NeedSignal == false || executor.Tags.Contains(SignalToken);
        }
    }
}