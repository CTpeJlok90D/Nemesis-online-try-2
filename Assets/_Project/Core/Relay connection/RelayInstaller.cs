using Unity.Services.Core;
using Zenject;

namespace Core.Conncetion
{
    public class RelayInstaller : MonoInstaller
    {
        private RelayConnection _relayConnection;

        public override void InstallBindings()
        { 
            UnityServices.InitializeAsync();
            
            _relayConnection = new();
            Container
                .Bind<RelayConnection>()
                .FromInstance(_relayConnection)
                .AsSingle();
        }
    }
}
