using Core.SelectionBase;
using Zenject;

namespace UI.Selection
{
    public class CoordinatesSelectionInstanceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CoordinatesSelection selection = Container.Resolve<CoordinatesSelection>();

            Container
                .Bind<ISelection>()
                .FromInstance(selection)
                .AsSingle();
        }
    }
}