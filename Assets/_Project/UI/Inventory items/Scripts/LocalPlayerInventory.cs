using System.Collections.Generic;
using System.Linq;
using Core.CharacterInventories;
using Core.PlayerTablets;
using UI.Common;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.InventoryItems
{
    [DefaultExecutionOrder(5)]
    public class LocalPlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemType _inventoryType;
        [SerializeField] private InventoryItemInstanceContainer _inventoryItemInstance_PREFAB;
        [SerializeField] private Transform _parent;

        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private NetworkManager _networkManager;

        private PlayerTablet LocalTablet => _playerTabletList.Local;
        
        private Inventory _inventory;
        private List<InventoryItemInstanceContainer> _instances;

        private void Awake()
        {
            _instances = new();
            UpdateLinkedInventory();
        }

        private void Update()
        {
            if (_networkManager.IsClient == false)
            {
                return;
            }
            
            if (_inventory == null)
            {
                UpdateLinkedInventory();
            }

            if (_inventory == null)
            {
                return;
            }
            
            SyncInventory();
        }

        private void UpdateLinkedInventory()
        {
            if (_inventoryType is ItemType.Big)
            {
                _inventory = LocalTablet.BigItemsInventory;
            }

            if (_inventoryType is ItemType.Small)
            {
                _inventory = LocalTablet.SmallItemsInventory;
            }
        }

        private void SyncInventory()
        {
            foreach (InventoryItem item in _inventory.ToArray())
            {
                while (_instances.Count(x => x.Item == item) > _inventory.Count(x => x == item))
                {
                    InventoryItemInstanceContainer instanceToRemove = _instances.First(x => x.Item == item);
                    _instances.Remove(instanceToRemove);

                    if (instanceToRemove.TryGetComponent(out IDestroyable destroyable))
                    {
                        destroyable.Destroy();
                    }
                    else
                    {
                        Destroy(instanceToRemove);
                    }
                }
                
                while (_instances.Count(x => x.Item == item) < _inventory.Count(x => x == item))
                {
                    InventoryItem itemToAdd = _inventory.First(x => x == item);
                    
                    InventoryItemInstanceContainer inventoryItemInstance = _inventoryItemInstance_PREFAB.Instantiate(itemToAdd, _parent);
                    _instances.Add(inventoryItemInstance);
                }
            }
        }
    }
}
