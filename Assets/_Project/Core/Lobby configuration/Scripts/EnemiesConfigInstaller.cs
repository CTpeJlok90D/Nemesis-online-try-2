using Core.Maps;
using Core.Maps.Generation;
using Zenject;

namespace Core.Lobbies
{
    public class EnemiesConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();
            EnemiesConfig enemiesConfig = lobbyInstaller.Lobby.Configuration.EnemiesConfig;
            
            Container.BindInstance(enemiesConfig).AsSingle();
        }
    }
}
