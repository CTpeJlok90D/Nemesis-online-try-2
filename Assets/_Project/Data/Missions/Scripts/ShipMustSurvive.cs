using Core;
using Core.Maps;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Ship must survive")]
    public class ShipMustSurvive : MissionTarget
    {
        [SerializeField] private bool _invert;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            bool result = Ship.Instance.IsDestroyed == false;
            if (_invert)
            {
                result = !result;
            }

            return result;
        }
    }
}