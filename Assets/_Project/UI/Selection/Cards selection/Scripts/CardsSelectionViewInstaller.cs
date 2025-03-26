using System.Diagnostics.Contracts;
using Core.Selection.Cards;
using Zenject;

namespace SelectionStarted
{
    public class CardsSelectionViewInstaller : MonoInstaller<CardsSelectionViewInstaller>
    {
        public override void InstallBindings()
        {
            CardsSelection cardsSelection = Container.Resolve<CardsSelection>();
            CardsSelectionView cardsSelectionView = new(cardsSelection);
            
            Container.Bind<CardsSelectionView>().FromInstance(cardsSelectionView).AsSingle();
        }
    }
}