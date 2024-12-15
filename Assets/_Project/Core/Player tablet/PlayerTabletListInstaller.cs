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

        public PlayerTabletList PlayerTabletList { get; private set; }

        public override void InstallBindings()
        {
            PlayerTabletList = _playerTabletList.Init(_activatorInstaller.Activator, _networkManagerInstaller.Instance);
            DontDestroyOnLoad(PlayerTabletList);

            Container
                .Bind<PlayerTabletList>()
                .FromInstance(PlayerTabletList)
                .AsSingle();
        }
    }
}
