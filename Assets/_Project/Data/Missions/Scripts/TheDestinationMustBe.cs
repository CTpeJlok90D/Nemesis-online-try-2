using Core;
using Core.Maps;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "The destination must be")]
    public class TheDestinationMustBe : MissionTarget
    {
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return Ship.Instance.Destination == TargetDestination && Ship.Instance.IsDestroyed == false;
        }
    }
}
