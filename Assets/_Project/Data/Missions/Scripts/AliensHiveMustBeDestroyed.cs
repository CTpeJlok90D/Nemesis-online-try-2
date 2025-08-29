using Core;
using Core.AliensTablets;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Aliens hive must be destroyed")]
    public class AliensHiveMustBeDestroyed : MissionTarget
    {
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return AliensTablet.Instance.EggCount.Value == 0;
        }
    }
}