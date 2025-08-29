using System.Linq;
using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Player N must be dead")]
    public class PlayerNMustBeDead : MissionTarget
    {
        [SerializeField] private int PlayerNumber;

        protected override bool IsConditionsAreMet(PlayerTablet executor)
        { 
            return 
                PlayerTablet.ActiveTablets.Any(x => x.OrderNumber.Value == PlayerNumber) == false;
        } 
    }
}