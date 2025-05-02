using Core.EventsDecks;
using Zenject;

namespace Core.Lobbies
{
    public class EventDeckConfigurationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();
            Lobby lobby = lobbyInstaller.Lobby;

            EventDeckConfiguration eventDeckConfiguration = lobby.Configuration.EventDeckConfiguration;
            Container
                .Bind<EventDeckConfiguration>()
                .FromInstance(eventDeckConfiguration);
        }
    }
}