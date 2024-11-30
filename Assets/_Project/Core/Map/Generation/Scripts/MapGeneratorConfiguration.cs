using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.AliensTablets;
using Core.DestinationCoordinats;
using Core.Map.IntellegenceTokens;
using Core.TabletopRandom;
using Unity.Netcode;

namespace Core.Maps.Generation
{
    [Serializable]
    public struct MapGeneratorConfiguration : INetworkSerializable
    {
        public const int WEAKNESS_CARDS_COUNT = 3;

        [SerializedDictionary("Layer","Bag")]
        public SerializedDictionary<int, Bag<RoomContent>> BagsOfRooms;

        public IntelegenceToken[] IntelegenceTokens;

        public DestinationCoordinatsCard[] AvailableDestinationCards;

        public AlienWeaknessCard[] AlienWeaknessCards;

        public int[] EscapePodCountPerPlayer;

        public int PlayerCount;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerCount);
            serializer.SerializeValue(ref AvailableDestinationCards);
            serializer.SerializeValue(ref IntelegenceTokens);
            serializer.SerializeValue(ref EscapePodCountPerPlayer);
            serializer.SerializeValue(ref AlienWeaknessCards);
            SerializeRooms(serializer);
        }

        private void SerializeRooms<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
             int count = 0;

            if (serializer.IsWriter)
            {
                count = BagsOfRooms.Count;
            }

            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                BagsOfRooms.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                int layer = 0;
                if (serializer.IsWriter)
                {
                    layer = BagsOfRooms.Keys.ElementAt(i);
                }
                serializer.SerializeValue(ref layer);

                int roomsCount = 0;
                if (serializer.IsWriter)
                {
                    roomsCount = BagsOfRooms.Values.ElementAt(i).Items.Count;
                }

                serializer.SerializeValue(ref roomsCount);

                RoomContent[] roomContents = new RoomContent[roomsCount];
                if (serializer.IsWriter)
                {
                    roomContents = BagsOfRooms.Values.ElementAt(i).Items.ToArray();
                }

                serializer.SerializeValue(ref roomContents);
                
                if (serializer.IsReader)
                {
                    BagsOfRooms.Add(layer, new(roomContents));
                }
            }
        }

        private void SerializeIntelegenceTokens<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;
            if (serializer.IsWriter)
            {
                count = IntelegenceTokens.Length;
            }

            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                IntelegenceTokens = new IntelegenceToken[count];
            }

            for (int i = 0; i < count; i++)
            {
                serializer.SerializeValue(ref IntelegenceTokens[i]);
            }
        }
    }
}
