using Core.CharacterInventories;
using UnityEngine;

namespace UI.Selection.InventoryItemsSelections
{
    public class InventoryItemSelectionItem : MonoBehaviour, IContainsInventoryItemInstance
    {
        public InventoryItem Item { get; private set; }
        
        public InventoryItemsSelectionItemsSpawner Spawner { get; private set; }

        public InventoryItemSelectionItem Instantiate(InventoryItem item, InventoryItemsSelectionItemsSpawner itemsSpawner, Transform parent)
        {
            gameObject.SetActive(false);
            InventoryItemSelectionItem result = Instantiate(this, parent);
            gameObject.SetActive(true);

            return result.Init(item, itemsSpawner);
        }

        public InventoryItemSelectionItem Init(InventoryItem item, InventoryItemsSelectionItemsSpawner itemsSpawner)
        {
            Item = item;
            Spawner = itemsSpawner;
            gameObject.SetActive(true);

            return this;
        }
    }
}
