using Core.Lobbies;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.CharacterChoose
{
    public class CharactersDealerInstaller : MonoInstaller
    {
        [SerializeField] private CharactersDealer _dealer;

        public CharactersDealer CharacterDealer => _dealer;

        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();
            PlayerTabletListInstaller playerTabletList = ProjectContext.Instance.GetComponentInChildren<PlayerTabletListInstaller>();

            _dealer.Init(lobbyInstaller.Lobby, playerTabletList.PlayerTabletList);

            Container.Bind<CharactersDealer>().FromInstance(_dealer).AsSingle();
        }
    }
}
