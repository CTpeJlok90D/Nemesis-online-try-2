using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.Network
{
    public class NetworkManagerInstaller : MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManager_PREFAB;

        public NetworkManager Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = Instantiate(_networkManager_PREFAB);

            Container.Bind<NetworkManager>()
                .FromInstance(Instance)
                .AsSingle();
        }

        public override void Start()
        {
            base.Start();
            DontDestroyOnLoad(Instance);
        }
    }
}
