using Core.Entities;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.CharacterInventories
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    public class InventoryItem : NetEntity<InventoryItem>
    {
        public NetBehaviourReference<Inventory> OwnerInventory;
        
        [SerializeField] private ItemType _itemType;
        [SerializeField] public string _id;
        public string ID => _id;
        
        public ItemType ItemType => _itemType;

        private void Awake()
        {
            OwnerInventory = new();
        }
    }
}
