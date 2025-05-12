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

        public IReadOnlyCollection<InventoryItem> GetItems()
        {
            return _items.ToEnumerable<InventoryItem>().ToArray();
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

        public void AddItem(InventoryItem item_PREFAB)
        {
            if (CanAddItem(item_PREFAB) == false)
            {
                throw new Exception("Can't add item");
            }

            InventoryItem item = Instantiate(item_PREFAB);
            item.NetworkObject.Spawn();
            AddItemInstance(item);
        }
        
        public void AddItemsInstancesRange(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                AddItemInstance(item);
            }
        }

        public void AddItemInstance(InventoryItem instance)
        {
            _items.Add(instance.NetworkObject);
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
            private Dictionary<string, string> _loadedItems = new();
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (Application.IsPlaying(target) == false)
                {
                    return;
                }
                
                GUILayout.Label("Inventory:");
                
                foreach (InventoryItem item in Inventory.GetItems())
                {
                    if (_loadedItems.ContainsKey(item.ID) == false)
                    {
                        _loadedItems.Add(item.ID, "");
                        _ = LoadItem(item.ID);
                    }
                }
                
                foreach (var item in _loadedItems)
                {
                    GUILayout.Label($"[{item.Value}]:[{item.Key}]");
                }
                
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                _itemName = EditorGUILayout.TextField(_itemName);
                if (GUILayout.Button("Add"))
                {
                    AddItem(_itemName);
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

            private async UniTask LoadItem(string guiID)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(guiID);
                await handle.ToUniTask();
                
                InventoryItem item = handle.Result.GetComponent<InventoryItem>();
                _loadedItems[guiID] = item.name;
            }
        }
#endif
    }
}
