using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Survive mission")]
    public class SurviveMission : MissionTarget
    {
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return executor.IsDead == false;
        }
    }
}