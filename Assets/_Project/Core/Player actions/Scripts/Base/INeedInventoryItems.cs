using Core.CharacterInventories;
using Core.Selection.InventoryItems;
using Cysharp.Threading.Tasks;

namespace Core.PlayerActions.Base
{
    public interface INeedInventoryItems
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
