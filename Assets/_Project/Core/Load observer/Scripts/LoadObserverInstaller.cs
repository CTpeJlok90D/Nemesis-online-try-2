using Core.Network;
using UnityEngine;
using Zenject;

namespace Core.LoadObservers
{
    public class LoadObserverInstaller : MonoInstaller
    {
        [SerializeField] private LoadObserver _loadObserver_PREFAB;
        
        [SerializeField] private NetworkManagerInstaller _networkManagerInstaller;

        public LoadObserver CharacterDealer { get; private set; }

        public override void InstallBindings()
        {
            CharacterDealer = _loadObserver_PREFAB.Instantiate(_networkManagerInstaller.Instance);
            DontDestroyOnLoad(CharacterDealer);

            Container
                .Bind<LoadObserver>()
                .FromInstance(CharacterDealer)
                .AsSingle();
        }
    }
}
