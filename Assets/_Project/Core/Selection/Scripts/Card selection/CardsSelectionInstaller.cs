using Zenject;

namespace Core.Selection.Cards
{
    public class CardsSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CardsSelection cardsSelection = new();
            Container.Bind<CardsSelection>().FromInstance(cardsSelection).AsSingle();
        }
    }
}
