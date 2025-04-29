using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AYellowpaper.SerializedCollections;
using TNRD;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.CharacterInventorys
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    [CreateAssetMenu(menuName = "Game/Inventory/Item")]
    public class InventoryItem : ScriptableObject, INetworkSerializable, IEquatable<InventoryItem>, INetScriptableObjectArrayElement<InventoryItem>
    {
        [SerializeField] private NetScriptableObject<InventoryItem> _netScriptableObject;
        [SerializeField] private SerializableInterface<IItemData>[] _additionalData;
        [SerializeField] private ItemType _itemType;
        public NetScriptableObject<InventoryItem> Net => _netScriptableObject;
        public string ID => Net.RuntimeLoadKey;
        
        public IReadOnlyCollection<IItemData> AdditionalData => _additionalData.Select(x => x.Value).ToArray();
        internal SerializedDictionary<string, string> StartItemInstanceData
        {
            get
            {
                SerializedDictionary<string, string> startItemInstanceData = new();

                foreach (IItemData itemData in AdditionalData)
                {
                    foreach (KeyValuePair<string, string> i in itemData.StartItemData)
                    {
                        if (startItemInstanceData.ContainsKey(i.Key))
                        {
                            throw new Exception($"Duplicate key {i.Key} in startItemData");
                        }
                        
                        startItemInstanceData.Add(i.Key, i.Value);
                    }
                }
                
                return startItemInstanceData;
            }
        }
        
        public ItemType ItemType => _itemType;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Net.Loaded += OnLoadEnd;
            Net.OnNetworkSerialize(serializer, this);
        }

        private void OnLoadEnd(InventoryItem result)
        {
            Net.Loaded -= OnLoadEnd;
            result._additionalData = _additionalData;
        }

        public bool Equals(InventoryItem other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _itemType == other._itemType;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InventoryItem)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), (int)_itemType);
        }
        
#if UNITY_EDITOR
        private IItemData[] _lastData = {};
        private void OnValidate()
        {
            EditorApplication.delayCall += DelayValidate;
        }

        private void DelayValidate()
        {
            foreach (SerializableInterface<IItemData> itemData in _additionalData)
            {
                if (itemData.Value is ScriptableObject scriptableObject)
                {
                    if (AssetDatabase.GetAssetPath(scriptableObject) == AssetDatabase.GetAssetPath(this))
                    {
                        continue;
                    }
                    
                    string path = AssetDatabase.GetAssetPath(scriptableObject);
                    AssetDatabase.RemoveObjectFromAsset(scriptableObject);
                    
                    AssetDatabase.DeleteAsset(path);
                    scriptableObject.name = scriptableObject.GetType().Name;
                    AssetDatabase.AddObjectToAsset(scriptableObject, this);
                    
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            foreach (IItemData item in _lastData)
            {
                if (_additionalData.Any(x => x.Value == item) == false)
                {
                    if (item is ScriptableObject scriptableObject)
                    {
                        AssetDatabase.RemoveObjectFromAsset(scriptableObject);
                        
                        string assetPath = AssetDatabase.GetAssetPath(this);
                        string folder = Path.Combine(Path.GetDirectoryName(assetPath), scriptableObject.GetType().Name + ".asset");
                        scriptableObject.name = Path.GetFileNameWithoutExtension(folder);
                        
                        AssetDatabase.CreateAsset(scriptableObject, folder);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            
            _lastData = _additionalData.Select(x => x.Value).ToArray();
        }
#endif
    }
}
