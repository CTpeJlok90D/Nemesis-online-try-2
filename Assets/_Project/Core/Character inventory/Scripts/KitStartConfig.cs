using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.Characters;
using Unity.Netcode;

namespace Core.CharacterInventorys
{
    [Serializable]
    public struct KitStartConfig : INetworkSerializable
    {
        public SerializedDictionary<Character, InventoryItem[]> StartItems;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;

            if (serializer.IsWriter)
            {
                count = StartItems.Count;
            }

            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                StartItems ??= new();
                StartItems.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                Character character = null;
                if (serializer.IsWriter)
                {
                    character = StartItems.Keys.ElementAt(i);
                }
                serializer.SerializeValue(ref character);

                InventoryItem[] characterStartItems = {};
                if (serializer.IsWriter)
                {
                    characterStartItems = StartItems[character];
                }
                serializer.SerializeValue(ref characterStartItems);

                if (serializer.IsReader)
                {
                    StartItems.Add(character, characterStartItems);
                }
            }
        }
    }
}
