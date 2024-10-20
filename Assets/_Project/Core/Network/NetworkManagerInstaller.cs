using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.Network
{
    public class NetworkManagerInstaller : MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManager;
        public override void InstallBindings()
        {
            if (NetworkManager.Singleton == null) 
            {
                Container.Bind<NetworkManager>().FromInstance(_networkManager);
            }
            else 
            {
                Container.Bind<NetworkManager>().FromInstance(NetworkManager.Singleton);
            }
        }
    }
}
