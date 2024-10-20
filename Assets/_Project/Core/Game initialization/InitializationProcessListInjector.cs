using Zenject;

namespace Core.GameInitialization
{
    public class InitializationProcessListInjector : MonoInstaller
    {
        public override void InstallBindings()
        {
            InitializationProcessList processList = new();
            Container.Bind<InitializationProcessList>().FromInstance(processList).AsSingle();
        }
    }
}
