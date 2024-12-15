using Core.Network;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private Lobby _lobby_PREFAB;
        
        [SerializeField] private PlayerTabletListInstaller _playetTabletListInstaller;

        [SerializeField] private NetworkManagerInstaller _networkManagerInstaller;

        public Lobby Lobby { get; private set; }

        public override void InstallBindings()
        {
            Lobby = _lobby_PREFAB.Instantiate(_playetTabletListInstaller.PlayerTabletList, _networkManagerInstaller.Instance);
            DontDestroyOnLoad(Lobby);

            Container
                .Bind<Lobby>()
                .FromInstance(Lobby)
                .AsSingle();
        }
    }
}
