using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.Aliens;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.AddressableAssets;

namespace Core.Maps
{
    [Serializable]
    public struct EnemiesConfig : INetworkSerializable
    {
        [SerializedDictionary("Alien token", "Enemy prefab")]
        public SerializedDictionary<string, AssetReference> TypeOfEnemies;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            
        }
        
        private void SerializeRooms<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;

            if (serializer.IsWriter)
            {
                count = TypeOfEnemies.Count;
            }

            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                TypeOfEnemies ??= new();
                TypeOfEnemies.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                FixedString128Bytes alienType = string.Empty;
                if (serializer.IsWriter)
                {
                    alienType = TypeOfEnemies.Keys.ElementAt(i);
                }
                serializer.SerializeValue(ref alienType);

                FixedString128Bytes addressableKey = new();
                if (serializer.IsWriter)
                {
                    addressableKey = TypeOfEnemies.Values.ElementAt(i).RuntimeKey.ToString();
                }
                serializer.SerializeValue(ref addressableKey);
                
                AssetReferenceT<RoomContent> enemy = new(addressableKey.ToString());

                if (serializer.IsReader)
                {
                    TypeOfEnemies.Add(alienType.ToString(), enemy);
                }
            }
        }
    }
}