using Core.CharacterInventorys;
using UnityEngine;

namespace UI.Selection.InventoryItemsSelections
{
    public class InventoryItemSelectionItem : MonoBehaviour, IContainsInventoryItemInstance
    {
        public InventoryItemInstance Item { get; private set; }
        
        public InventoryItemsSelectionItemsSpawner Spawner { get; private set; }

        public InventoryItemSelectionItem Instantiate(InventoryItemInstance item, InventoryItemsSelectionItemsSpawner itemsSpawner, Transform parent)
        {
            gameObject.SetActive(false);
            InventoryItemSelectionItem result = Instantiate(this, parent);
            gameObject.SetActive(true);

            return result.Init(item, itemsSpawner);
        }

        public InventoryItemSelectionItem Init(InventoryItemInstance item, InventoryItemsSelectionItemsSpawner itemsSpawner)
        {
            Item = item;
            Spawner = itemsSpawner;
            gameObject.SetActive(true);

            return this;
        }
    }
}
