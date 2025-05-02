using Core.CharacterInventories;
using Core.SelectionBase;

namespace Core.Selection.InventoryItems
{
    public class InventoryItemsSelection : Selection<InventoryItem>
    {
        public override bool OnlyUniqueItems => false;
    }
}
