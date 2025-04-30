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
    [DefaultExecutionOrder(5)]
    public class LocalPlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemType _inventoryType;
        [SerializeField] private InventoryItemInstanceContainer _inventoryItemInstance_PREFAB;
        [SerializeField] private Transform _parent;

        [Inject] private PlayerTabletList _playerTabletList;

        private PlayerTablet localTablet => _playerTabletList.Local;
        
        private Inventory _inventory;
        private List<InventoryItemInstanceContainer> _instances;

        private void Awake()
        {
            _instances = new();
        }

        private void UpdateLinkedInventory()
        {
            if (_inventoryType is ItemType.Big)
            {
                _inventory = localTablet.BigItemsInventory;
            }

            if (_inventoryType is ItemType.Small)
            {
                _inventory = localTablet.SmallItemsInventory;
            }
        }

        private void OnEnable()
        {
            localTablet.PawnLinked += OnPawnLink;
            if (localTablet.CharacterPawn != null)
            {
                UpdateLinkedInventory();
                _inventory.ItemsListChanged += OnItemListChange;
                SyncInventory();
            }
        }

        private void OnDisable()
        {
            // if (localTablet != null && localTablet.CharacterPawn != null)
            // {
            //     localTablet.PawnLinked -= OnPawnLink;
            //     _inventory.ItemsListChanged -= OnItemListChange;
            // throwing error for now.
            // }
        }

        private void OnPawnLink(PlayerTablet sender)
        {
            if (_inventory != null)
            {
                _inventory.ItemsListChanged -= OnItemListChange;
            }
            UpdateLinkedInventory();
            _inventory.ItemsListChanged += OnItemListChange;
            
            SyncInventory();
        }

        private void OnItemListChange(Inventory sender)
        {
            SyncInventory();
        }

        private void SyncInventory()
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
