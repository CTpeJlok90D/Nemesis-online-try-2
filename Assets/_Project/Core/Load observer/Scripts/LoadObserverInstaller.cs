using Core.Network;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.LoadObservers
{
    public class LoadObserverInstaller : MonoInstaller
    {
        [SerializeField] private LoadObserver _loadObserver_PREFAB;

        public LoadObserver LoadObserver { get; private set; }

        public override void InstallBindings()
        {
            LoadObserver = _loadObserver_PREFAB.Instantiate();
            DontDestroyOnLoad(LoadObserver);

            Container
                .Bind<LoadObserver>()
                .FromInstance(LoadObserver)
                .AsSingle();
        }
    }
}
