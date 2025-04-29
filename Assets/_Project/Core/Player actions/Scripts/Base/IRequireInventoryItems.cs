using System;
using Core.CharacterInventorys;
using Core.Selection.InventoryItems;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Custom;

namespace Core.PlayerActions.Base
{
    public interface IRequireInventoryItems
    {
        public int RequiredItemsAmount { get; }
        
        public InventoryItemInstance[] InventoryItemsSource { get; }
        
        public InventoryItemInstance[] InventoryItemsSelection { get; set; }

        internal async UniTask<InventoryItemInstance[]> GetSelectionLocal(InventoryItemsSelection selection, NetVariable<bool> actionIsExecuting)
        {
            InventoryItemInstance[] selected = await selection.SelectFrom(InventoryItemsSource, RequiredItemsAmount);
            
            if (selected.Length != RequiredItemsAmount)
            {
                actionIsExecuting.Value = false;
                InventoryItemsSelection = Array.Empty<InventoryItemInstance>(); 
                return Array.Empty<InventoryItemInstance>();
            }

            InventoryItemsSelection = selected;
            return selected;
        }
    }
}
