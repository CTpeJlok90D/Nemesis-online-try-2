using Core.Starter;
using UnityEngine;
using Zenject;

namespace Core.PlayerTablets
{
    public class PlayerTabletListInstaller : MonoInstaller
    {
        [SerializeField] private PlayerTabletList _playerTabletList_PREFAB;
        [SerializeField] private ActivatorInstaller _activatorInstaller;

        public PlayerTabletList Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = _playerTabletList_PREFAB.Instantiate(_activatorInstaller.Activator);
            DontDestroyOnLoad(Instance);

            Container
                .Bind<PlayerTabletList>()
                .FromInstance(Instance)
                .AsSingle();
        }
    }
}
