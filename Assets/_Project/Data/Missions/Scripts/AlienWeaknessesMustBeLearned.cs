using System.Linq;
using Core;
using Core.AliensTablets;
using Core.Missions;
using Core.PlayerTablets;
using UnityEngine;

namespace Data.Missions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Missions + "Alien weaknesses must be learned")]
    public class AlienWeaknessesMustBeLearned : MissionTarget
    {
        [SerializeField] private AlienWeaknessType[] TypesToUnlock;
        protected override bool IsConditionsAreMet(PlayerTablet executor)
        {
            AliensTablet tablet = AliensTablet.Instance;

            bool isAllUnlocked = TypesToUnlock.All(x => tablet.UnlockedWeaknessTypes.Contains(x));
            return isAllUnlocked;
        }
    }
}