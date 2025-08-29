using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Const mission")]
    public class ConstValue : MissionTarget
    {
        [SerializeField] private bool _missionValue;
        [SerializeField] private bool _surviveValue;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return _missionValue;
        }

        public override bool IsSurvived(PlayerTablet executor)
        {
            return _surviveValue;
        }
    }
}
