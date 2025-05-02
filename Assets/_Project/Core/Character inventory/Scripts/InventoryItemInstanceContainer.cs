using UnityEngine;

namespace Core.CharacterInventories
{
    public class InventoryItemInstanceContainer : MonoBehaviour, IContainsInventoryItemInstance
    {
        public InventoryItem InventoryItemInstance { get; private set; }

        public InventoryItem Item => InventoryItemInstance;
        
        public InventoryItemInstanceContainer Instantiate(InventoryItem inventoryItemInstance, Transform parent)
        {
            gameObject.SetActive(false);
            InventoryItemInstanceContainer instance = Instantiate(this, parent);
            gameObject.SetActive(true);

            return instance.Init(inventoryItemInstance);
        }

        public InventoryItemInstanceContainer Init(InventoryItem inventoryItemInstance)
        {
            InventoryItemInstance = inventoryItemInstance;
            gameObject.SetActive(true);
            return this;
        }
    }
}
