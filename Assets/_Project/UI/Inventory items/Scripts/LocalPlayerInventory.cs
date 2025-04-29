using System;
using System.Collections.Generic;
using System.Linq;
using Core.CharacterInventorys;
using Core.PlayerTablets;
using UI.Common;
using UnityEngine;
using Zenject;

namespace UI.InventoryItems
{
    public class LocalPlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemType _inventoryType;
        [SerializeField] private InventoryItemInstanceContainer _inventoryItemInstance_PREFAB;
        [SerializeField] private Transform _parent;

        [Inject] private PlayerTabletList _playerTabletList;
        
        private Inventory _inventory;
        private List<InventoryItemInstanceContainer> _instances;

        private void Awake()
        {
            _instances = new();
            
            if (_inventoryType is ItemType.Big)
            {
                _inventory = _playerTabletList.Local.BigItemsInventory;
            }

            if (_inventoryType is ItemType.Small)
            {
                _inventory = _playerTabletList.Local.SmallItemsInventory;
            }
        }

        private void OnEnable()
        {
            _inventory.ItemsListChanged += OnItemListChange;
        }

        private void OnDisable()
        {
            _inventory.ItemsListChanged -= OnItemListChange;
        }

        private void OnItemListChange(Inventory sender)
        {
            ValidateInventory();
        }

        private void ValidateInventory()
        {
            foreach (InventoryItemInstance item in _inventory.ToArray())
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
                    InventoryItemInstance itemToAdd = _inventory.First(x => x == item);
                    
                    InventoryItemInstanceContainer inventoryItemInstance = _inventoryItemInstance_PREFAB.Instantiate(itemToAdd, _parent);
                    _instances.Add(inventoryItemInstance);
                }
            }
        }
    }
}
