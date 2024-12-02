using System;
using Unity.Netcode;

namespace Core.EventsDeck
{
    [Serializable]
    public struct EventDeckConfiguration : INetworkSerializable
    {
        public EventCard[] Deck;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Deck);
        }
    }
}