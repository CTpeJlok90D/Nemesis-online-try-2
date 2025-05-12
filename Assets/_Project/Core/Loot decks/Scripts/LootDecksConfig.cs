using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.CharacterInventories;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.AddressableAssets;

namespace Core.LootDecks
{
    [Serializable]
    public struct LootDecksConfig : INetworkSerializable
    {
        public SerializedDictionary<LootDeck.Type, AssetReference[]> DeckItems;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;

            if (serializer.IsWriter)
            {
                count = DeckItems.Count;
            }

            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                DeckItems ??= new();
                DeckItems.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                LootDeck.Type lootType = LootDeck.Type.BattleDeck;
                if (serializer.IsWriter)
                {
                    lootType = DeckItems.Keys.ElementAt(i);
                }
                serializer.SerializeValue(ref lootType);

                FixedString128Bytes[] characterStartItems = {};
                if (serializer.IsWriter)
                {
                    characterStartItems = DeckItems[lootType].Select(x => new FixedString128Bytes(x.RuntimeKey.ToString())).ToArray();
                }

                int itemsCount = 0;
                if (serializer.IsWriter)
                {
                    itemsCount = DeckItems.Count;
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
                    DeckItems.Add(lootType, characterStartItems.Select(x => new AssetReference(x.ToString())).ToArray());
                }
            }
        }
    }
}
