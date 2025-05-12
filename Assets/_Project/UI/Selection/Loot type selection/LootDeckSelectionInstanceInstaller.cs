using Core.Selection.LootDeckSelections;
using Core.SelectionBase;
using Zenject;

namespace UI.Selection.LootTypeSelections
{
    public class LootDeckSelectionInstanceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LootDeckSelection lootDeckSelection = Container.Resolve<LootDeckSelection>();
            Container.Bind<ISelection>().FromInstance(lootDeckSelection);
        }
    }
}
