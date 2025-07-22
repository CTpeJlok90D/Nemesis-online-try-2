using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.AddressableAssets;
#endif

namespace Core.CharacterInventories
{
    [Icon("Assets/_Project/Core/Character inventory/Editor/icons8-expedition-backpack-96.png")]
    public class Inventory : NetworkBehaviour, IEnumerable<InventoryItem>
    {
        [SerializeField] private int _limit;
        [SerializeField] private ItemType[] _acceptebleItemTypes;
        public IReadOnlyCollection<ItemType> AcceptableItemTypeTypes => _acceptebleItemTypes;
        
        private NetworkList<NetworkObjectReference> _items;

        public delegate void ItemsListChangedDelegate(Inventory sender);
        public event ItemsListChangedDelegate ItemsListChanged;
        
        public IReadOnlyCollection<InventoryItem> Items => _items.ToEnumerable<InventoryItem>().ToArray();
        
        private void Awake()
        {
            _items = new();
        }

        private void OnEnable()
        {
            _items.OnListChanged += OnListChange;
        }

        private void OnDisable()
        {
            _items.OnListChanged -= OnListChange;
        }

        private void OnListChange(NetworkListEvent<NetworkObjectReference> changeEvent)
        {
            OnListChange();
        }

        private void OnListChange()
        {
            ItemsListChanged?.Invoke(this);
        }
        
        public bool CanAddItem(InventoryItem item)
        {
            return AcceptableItemTypeTypes.Contains(item.ItemType);
        }
        
        public void AddItemsRange(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                AddItem(item);
            }
        }

        public void AddItem(InventoryItem itemToAdd)
        {
            if (CanAddItem(itemToAdd) == false)
            {
                throw new Exception("Can't add item");
            }

            if (itemToAdd.IsInstance)
            {
                AddItemInstance(itemToAdd);
                return;
            }

            InventoryItem item = Instantiate(itemToAdd);
            item.NetworkObject.Spawn();
            AddItemInstance(item);
        }
        
        private void AddItemsInstancesRange(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                AddItemInstance(item);
            }
        }

        private void AddItemInstance(InventoryItem instance)
        {
            _items.Add(instance.NetworkObject);
            instance.NetworkObject.TrySetParent(NetworkObject);
            instance.OwnerInventory.Reference = this;
        }

        public void RemoveItem(InventoryItem item)
        {
            _items.Remove(item.NetworkObject);
            item.OwnerInventory.Reference = null;
        }
        
        public IEnumerator<InventoryItem> GetEnumerator()
        {
            return _items.ToEnumerable<InventoryItem>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.ToEnumerable().GetEnumerator();
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(Inventory))]
        private class CEditor : Editor
        {
            private Inventory Inventory => target as Inventory;
            private string _itemName;
            private List<InventoryItem> _loadedItems = new();
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (Application.IsPlaying(target) == false)
                {
                    return;
                }
                
                GUILayout.Label("Inventory:");
                
                _loadedItems.Clear();
                foreach (InventoryItem item in Inventory.Items)
                {
                    if (_loadedItems.Contains(item) == false)
                    {
                        _loadedItems.Add(item);
                    }
                }
                
                GUI.enabled = false;
                foreach (InventoryItem item in _loadedItems)
                {
                    EditorGUILayout.ObjectField(item, typeof(InventoryItem), true);
                }
                GUI.enabled = true;
                
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                _itemName = EditorGUILayout.TextField(_itemName);
                if (GUILayout.Button("Add"))
                {
                    _ = AddItem(_itemName);
                }
                GUILayout.EndHorizontal();
            }

            private async UniTask AddItem(string itemName)
            {
                AsyncOperationHandle<InventoryItem> handle = Addressables.LoadAssetAsync<InventoryItem>(itemName);
                await handle.ToUniTask();
                
                InventoryItem item = handle.Result;
                Inventory.AddItem(item);
            }
        }
#endif
    }
}
