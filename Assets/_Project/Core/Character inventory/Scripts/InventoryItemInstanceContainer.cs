using UnityEngine;

namespace Core.CharacterInventorys
{
    public class InventoryItemInstanceContainer : MonoBehaviour, IContainsInventoryItemInstance
    {
        public InventoryItemInstance InventoryItemInstance { get; private set; }

        public InventoryItemInstance Item => InventoryItemInstance;
        
        public InventoryItemInstanceContainer Instantiate(InventoryItemInstance inventoryItemInstance, Transform parent)
        {
            gameObject.SetActive(false);
            InventoryItemInstanceContainer instance = Instantiate(this, parent);
            gameObject.SetActive(true);

            return instance.Init(inventoryItemInstance);
        }

        public InventoryItemInstanceContainer Init(InventoryItemInstance inventoryItemInstance)
        {
            InventoryItemInstance = inventoryItemInstance;
            gameObject.SetActive(true);
            return this;
        }
    }
}
