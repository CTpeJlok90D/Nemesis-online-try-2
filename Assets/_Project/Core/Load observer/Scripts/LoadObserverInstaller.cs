using Core.Network;
using UnityEngine;
using Zenject;

namespace Core.LoadObservers
{
    public class LoadObserverInstaller : MonoInstaller
    {
        [SerializeField] private LoadObserver _loadObserver_PREFAB;
        [SerializeField] private NetworkManagerInstaller _networkManagerInstaller;

        public LoadObserver Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = _loadObserver_PREFAB.Instantiate(_networkManagerInstaller.Instance);
            DontDestroyOnLoad(Instance);

            Container
                .Bind<LoadObserver>()
                .FromInstance(Instance)
                .AsSingle();
        }
    }
}
