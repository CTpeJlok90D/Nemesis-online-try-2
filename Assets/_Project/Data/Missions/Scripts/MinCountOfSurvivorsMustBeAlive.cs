using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Min other players must survive")]
    public class MinCountOfSurvivorsMustBeAlive : MissionTarget
    {
        [SerializeField] private int _count = 1;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return executor.IsDead == false && PlayerTablet.Instances.Count > _count;
        }
    }
}