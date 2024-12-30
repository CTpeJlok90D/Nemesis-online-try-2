using Core.ActionsCards;
using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class ActionDeckConfigurationInstaller : MonoInstaller
    {
        [SerializeField] private LobbyInstaller _lobbyInstaller;

        public override void InstallBindings()
        {
            Lobby lobby = _lobbyInstaller.Lobby;
            LobbyConfiguration configuration = lobby.Configuration;

            Container.Bind<ActionCardsDeck.Config>().FromInstance(configuration.ActionCardsDeckConfiguration);
        }
    }
}
