using Core.Selection.Cards;
using Core.SelectionBase;
using Zenject;

namespace SelectionStarted
{
    public class CardsSelectionInstanceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CardsSelection roomSelection = Container.Resolve<CardsSelection>();
            Container.Bind<ISelection>().FromInstance(roomSelection).AsSingle();
        }
    }
}
