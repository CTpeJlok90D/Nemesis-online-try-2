using System.Linq;
using Core;
using Core.Aliens;
using Core.AliensBags;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "All type of enemies must be dead")]
    public class AllTypeOfAliensMustBeDead : MissionTarget
    {
        [SerializeField] private AlienToken _alienType;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return AliensBag.Instance.AlienTokens.Any(x => x.AlienType == _alienType.AlienType) == false && 
                   Enemy.GetAllEnemiesWithType(_alienType).Any() == false;
        }
    }
}