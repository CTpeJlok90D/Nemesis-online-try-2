using Core.Maps.Generation;
using Zenject;

namespace Core.Lobbies
{
    public class MapConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();

            MapGeneratorConfiguration mapConfiguration = lobbyInstaller.Lobby.Configuration.MapGeneratorConfiguration;
            Container
                .Bind<MapGeneratorConfiguration>()
                .FromInstance(mapConfiguration);
        }
    }
}
