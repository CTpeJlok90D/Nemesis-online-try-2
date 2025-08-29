using Core;
using Core.AliensTablets;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "N alien weaknesses must be learned")]
    public class NAlienWeaknessesMustBeLearned : MissionTarget
    {
        [SerializeField] private int AlienWeaknessLearnCountToWin = 2;

        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            return AliensTablet.Instance.UnlockedWeaknessTypes.Count >= AlienWeaknessLearnCountToWin;
        }
    }
}