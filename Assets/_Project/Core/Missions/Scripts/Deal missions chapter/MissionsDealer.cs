using System.Linq;
using Core.PlayerTablets;
using Core.Scenarios;
using Core.TabletopRandom;
using UnityEngine;
using Zenject;

namespace Core.Missions.Dealing
{
    public class MissionsDealer : MonoBehaviour, IChapter
    {
        [Inject] private MissionsDealerConfiguration _config;

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            Mission[] availableMissions = _config.AvailableMissions.ToArray();

            Bag<Mission> personalMissions = new(availableMissions.Where(x => x.Type == MissionType.Personal && x.MinPlayerCount <= PlayerTablet.Instances.Count));
            Bag<Mission> corporateMissions = new(availableMissions.Where(x => x.Type == MissionType.Сorporate && x.MinPlayerCount <= PlayerTablet.Instances.Count));

            foreach (PlayerTablet tablet in PlayerTablet.Instances)
            {
                int personalMissionsLeft = _config.PersonalMissionsCount;
                int corporateMissionsLeft = _config.CorporateMissionsCount;

                while (personalMissionsLeft > 0)
                {
                    Mission mission = personalMissions.PickOne();
                    tablet.Missions.Add(mission);
                    personalMissionsLeft--;

                    if (personalMissions.Items.Count == 0)
                    {
                        personalMissions = new(availableMissions.Where(x => x.Type == MissionType.Personal && x.MinPlayerCount <= PlayerTablet.Instances.Count));
                        Debug.LogWarning("[<color=yellow>Missions dealing</color>] Personal missions is out. Duplication is possible");
                    }
                }

                while (corporateMissionsLeft > 0)
                {
                    Mission mission = corporateMissions.PickOne();
                    tablet.Missions.Add(mission);
                    corporateMissionsLeft--;

                    if (corporateMissions.Items.Count == 0)
                    {
                        corporateMissions = new(availableMissions.Where(x => x.Type == MissionType.Сorporate && x.MinPlayerCount <= PlayerTablet.Instances.Count));
                        Debug.LogWarning("[<color=yellow>Missions dealing</color>] Сorporate missions is out. Duplication is possible");
                    }
                }
            }
            Ended?.Invoke(this);
        }
    }
}
