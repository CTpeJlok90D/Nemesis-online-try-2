using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.Characters;
using Unity.Collections;
using Unity.Netcode;

namespace Core.CharacterInventories
{
    [Serializable]
    public struct KitStartConfig : INetworkSerializable
    {
        public SerializedDictionary<Character, string[]> StartItems;
        
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

                FixedString128Bytes[] characterStartItems = {};
                if (serializer.IsWriter)
                {
                    characterStartItems = StartItems[character].Select(x => new FixedString128Bytes(x)).ToArray();
                }

                int itemsCount = 0;
                if (serializer.IsWriter)
                {
                    itemsCount = StartItems.Count;
                }

                if (serializer.IsReader)
                {
                    characterStartItems = new FixedString128Bytes[itemsCount];
                }
                
                serializer.SerializeValue(ref itemsCount);

                for (int j = 0; j < itemsCount; j++)
                {
                    serializer.SerializeValue(ref characterStartItems[i]);
                }

                if (serializer.IsReader)
                {
                    StartItems.Add(character, characterStartItems.Select(x => x.ToString()).ToArray());
                }
            }
        }
    }
}
