using System.Linq;
using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "And")]
    public class And : MissionTarget
    {
        [SerializeField] private MissionTarget[] _targets;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return _targets.All(x => x.IsCompletedFor(executor));
        }
    }
}