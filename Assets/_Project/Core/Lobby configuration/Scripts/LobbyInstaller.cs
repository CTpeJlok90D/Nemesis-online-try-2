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

        public Lobby Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = _lobby_PREFAB.Instantiate(_playetTabletListInstaller.Instance, _networkManagerInstaller.Instance);
            DontDestroyOnLoad(Instance);

            Container
                .Bind<Lobby>()
                .FromInstance(Instance)
                .AsSingle();
        }
    }
}
