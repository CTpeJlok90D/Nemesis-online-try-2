using Core.Missions.Dealing;
using Zenject;

namespace Core.Lobbies
{
    public class DealMissionsConfigurationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();

            MissionsDealerConfiguration missonDealerConfiguration = lobbyInstaller.Lobby.Configuration.DealMissionsConfiguration;
            Container.Bind<MissionsDealerConfiguration>().FromInstance(missonDealerConfiguration);
        }
    }
}
