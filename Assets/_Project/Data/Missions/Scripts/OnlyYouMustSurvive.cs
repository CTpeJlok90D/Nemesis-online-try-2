using System.Linq;
using Core;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Only you must survive")]
    public class OnlyYouMustSurvive : MissionTarget
    {
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return PlayerTablet.ActiveTablets.Count == 1 && PlayerTablet.ActiveTablets.First(x => x == executor);
        }
    }
}