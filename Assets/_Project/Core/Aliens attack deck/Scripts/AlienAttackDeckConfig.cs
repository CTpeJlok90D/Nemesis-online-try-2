using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.AlienAttackDecks
{
    [Serializable]
    public struct AlienAttackDeckConfig : INetworkSerializable
    {
        public AlienAttackCard[] Cards;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Cards);
        }
    }
}
