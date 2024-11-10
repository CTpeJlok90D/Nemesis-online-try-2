using System;
using Core.Characters;
using Unity.Netcode;

namespace Core.Lobbies
{
    [Serializable]
    public struct LobbyConfiguration : INetworkSerializable
    {
        public int PlayersCount;   

        public int ChooseCharactersCount;
        
        public Character[] Characters;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayersCount);
            serializer.SerializeValue(ref ChooseCharactersCount);

            int arraySize = 0;
            if (serializer.IsWriter)
            {
                arraySize = Characters.Length;
            }

            serializer.SerializeValue(ref arraySize);
            if (serializer.IsReader)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    serializer.SerializeValue(ref Characters[i]);
                }
            }
        }
    }
}
