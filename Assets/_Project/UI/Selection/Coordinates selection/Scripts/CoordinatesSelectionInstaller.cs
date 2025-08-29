using Zenject;

namespace UI.Selection
{
    public class CoordinatesSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CoordinatesSelection selection = new();

            Container
                .Bind<CoordinatesSelection>()
                .FromInstance(selection)
                .AsSingle();
        }
    }
}