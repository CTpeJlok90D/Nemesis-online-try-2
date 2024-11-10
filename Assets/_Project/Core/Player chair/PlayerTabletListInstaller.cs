using UnityEngine;
using Zenject;

namespace Core.PlayerTablets
{
    public class PlayerTabletListInstaller : MonoInstaller
    {
        [SerializeField] private PlayerTabletList _playerTabletList_PREFAB;

        public PlayerTabletList Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = Instantiate(_playerTabletList_PREFAB);
            DontDestroyOnLoad(Instance);

            Container
                .Bind<PlayerTabletList>()
                .FromInstance(Instance)
                .AsSingle();
        }
    }
}
