using System;
using System.Collections.Generic;
using System.Linq;
using Core.CharacterInventories;
using Core.Selection.InventoryItems;
using Core.SelectionBase;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace UI.Selection.InventoryItemsSelections
{
    public class InventoryItemsSelectionItemsSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("_itemContainerPrefab")] [FormerlySerializedAs("_item_PREFAB")] [field: SerializeField] private InventoryItemSelectionItem _itemSelectionItemPrefab;
        [field: SerializeField] private Transform _parent;
        
        [Inject] private InventoryItemsSelection _selection;

        private List<InventoryItemSelectionItem> _inventoryItemInstances;
        private List<InventoryItemSelectionItem> _selectedItems;

        public IReadOnlyList<InventoryItemSelectionItem> SelectedItems => _selectedItems;
        
        private void Awake()
        {
            _inventoryItemInstances = new List<InventoryItemSelectionItem>();
            _selectedItems = new List<InventoryItemSelectionItem>();
        }

        private void OnEnable()
        {
            DestroyItems();
            InstantiateItems();
            _selection.SelectionChanged += OnSelectionChange;
        }

        private void OnDisable()
        {
            _selection.SelectionChanged -= OnSelectionChange;
        }

        private void OnSelectionChange(ISelection sender)
        {
            foreach (InventoryItemSelectionItem instance in _selectedItems.ToArray())
            {
                if (_selection.Count(x => x.ID == instance.Item.ID) < _selectedItems.Count(x => x.Item.ID == instance.Item.ID))
                {
                    DeselectItem(instance);
                }
            }
        }

        private void DestroyItems()
        {
            foreach (InventoryItemSelectionItem inventoryItemInstance in _inventoryItemInstances)
            {
                Destroy(inventoryItemInstance.gameObject);
            }
            _inventoryItemInstances.Clear();
        }
        
        private void InstantiateItems()
        {
            foreach (InventoryItem inventoryItem in _selection.SelectionSource)
            {
                InventoryItemSelectionItem instance = _itemSelectionItemPrefab.Instantiate(inventoryItem, this, _parent);
                _inventoryItemInstances.Add(instance);
            }
        }

        public void SelectItem(InventoryItemSelectionItem item)
        {
            if (_selection.Contains(item.Item) && _selection.Count == _selection.RequiredCount)
            {
                _selection.Remove(item.Item);
                InventoryItemSelectionItem itemSelectionItem = _selectedItems.Find(x => x.Item.ID == item.Item.ID);
                _selectedItems.Remove(itemSelectionItem);
            }
            
            _selectedItems.Add(item);
            _selection.Add(item.Item);
        }

        public void DeselectItem(InventoryItemSelectionItem item)
        {
            _selectedItems.Remove(item);
            _selection.Remove(item.Item);
        }
    }
}