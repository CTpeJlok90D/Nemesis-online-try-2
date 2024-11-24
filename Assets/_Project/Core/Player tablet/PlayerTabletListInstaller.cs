using Core.Network;
using Core.Starter;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.PlayerTablets
{
    public class PlayerTabletListInstaller : MonoInstaller
    {
        [SerializeField] private PlayerTabletList _playerTabletList;
        [SerializeField] private ActivatorInstaller _activatorInstaller;
        [SerializeField] private NetworkManagerInstaller _networkManagerInstaller;

        public PlayerTabletList Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = _playerTabletList.Init(_activatorInstaller.Activator, _networkManagerInstaller.Instance);
            DontDestroyOnLoad(Instance);

            Container
                .Bind<PlayerTabletList>()
                .FromInstance(Instance)
                .AsSingle();
        }
    }
}
