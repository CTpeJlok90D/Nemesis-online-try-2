using System;
using Core.CharacterInventories;
using Core.Selection.InventoryItems;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Custom;

namespace Core.PlayerActions.Base
{
    public interface IRequireInventoryItems
    {
        public int RequiredItemsAmount { get; }
        
        public InventoryItem[] InventoryItemsSource { get; }
        
        public InventoryItem[] InventoryItemsSelection { get; set; }

        internal async UniTask<InventoryItem[]> GetSelectionLocal(InventoryItemsSelection selection)
        {
            InventoryItem[] selected = await selection.SelectFrom(InventoryItemsSource, RequiredItemsAmount);

            InventoryItemsSelection = selected;
            return selected;
        }
    }
}
