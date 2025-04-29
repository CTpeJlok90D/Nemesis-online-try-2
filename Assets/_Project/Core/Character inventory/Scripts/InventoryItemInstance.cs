using System;
using AYellowpaper.SerializedCollections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core.CharacterInventorys
{
    [Serializable]
    public struct InventoryItemInstance : INetworkSerializable, IEquatable<InventoryItemInstance>
    {
        private FixedString128Bytes _itemID;
        public FixedString512Bytes DataJson;
        
        public string ID => _itemID.ToString();
        
        public InventoryItemInstance(InventoryItem item)
        {
            _itemID = item.name;
            SerializedDictionary<string, string> data = item.StartItemInstanceData;
            DataJson = JsonUtility.ToJson(data);
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _itemID);
            serializer.SerializeValue(ref DataJson);
        }

        public bool Equals(InventoryItemInstance other)
        {
            return _itemID.Equals(other._itemID) && DataJson.Equals(other.DataJson);
        }

        public override bool Equals(object obj)
        {
            return obj is InventoryItemInstance other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_itemID, DataJson);
        }

        public static bool operator ==(InventoryItemInstance left, InventoryItemInstance right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryItemInstance left, InventoryItemInstance right)
        {
            return !(left == right);
        }
    }
}
