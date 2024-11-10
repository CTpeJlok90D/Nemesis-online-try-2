using Core.Lobbies;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.CharacterChoose
{
    public class CharactersDealerInstaller : MonoInstaller
    {
        [SerializeField] private CharactersDealer _dealer;

        public CharactersDealer Instance => _dealer;

        public override void InstallBindings()
        {
            LobbyInstaller lobbyInstaller = ProjectContext.Instance.GetComponentInChildren<LobbyInstaller>();
            PlayerTabletListInstaller playerTabletList = ProjectContext.Instance.GetComponentInChildren<PlayerTabletListInstaller>();

            _dealer.Init(lobbyInstaller.Instance, playerTabletList.Instance);

            Container.Bind<CharactersDealer>().FromInstance(Instance).AsSingle();
        }
    }
}
