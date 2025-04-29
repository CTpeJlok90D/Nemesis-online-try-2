using UnityEngine;
using Zenject;

namespace Core.Selection.InventoryItems
{
    public class InventoryItemsSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InventoryItemsSelection selection = new();
            
            Container.BindInstance(selection).AsSingle();
        }
    }
}
