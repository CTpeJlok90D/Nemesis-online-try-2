using Core.CharacterInventorys;
using Core.SelectionBase;

namespace Core.Selection.InventoryItems
{
    public class InventoryItemsSelection : Selection<InventoryItemInstance>
    {
        public override bool OnlyUniqueItems => false;
    }
}
