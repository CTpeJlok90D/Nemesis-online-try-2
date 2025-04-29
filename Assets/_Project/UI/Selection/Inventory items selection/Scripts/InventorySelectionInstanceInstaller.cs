using Core.Selection.InventoryItems;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace UI.Selection.InventoryItemsSelections
{
    public class InventorySelectionInstanceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InventoryItemsSelection inventoryItemsSelectionItemsSpawner = Container.Resolve<InventoryItemsSelection>();
            
            Container.Bind<ISelection>().FromInstance(inventoryItemsSelectionItemsSpawner).AsSingle();
        }
    }
}
