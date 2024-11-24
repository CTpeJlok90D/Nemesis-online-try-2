using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.TabletopRandom;
using Unity.Netcode;
using UnityEngine;

namespace Core.Maps.Generation
{
    [Serializable]
    public struct MapGeneratorConfiguration : INetworkSerializable
    {
        [SerializedDictionary("Layer","Bag")]
        public SerializedDictionary<int, Bag<RoomContent>> BagsOfRooms;

        public int PlayerCount;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerCount);

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
    }
}
