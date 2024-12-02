using System;
using Core.Characters;
using Core.EventsDeck;
using Core.Maps.Generation;
using Core.Missions.Dealing;
using Unity.Netcode;

namespace Core.Lobbies
{
    [Serializable]
    public struct LobbyConfiguration : INetworkSerializable
    {
        public int PlayersCount;   

        public int ChooseCharactersCount;
        
        public Character[] Characters;

        public MapGeneratorConfiguration MapGeneratorConfiguration;

        public MissionsDealerConfiguration DealMissionsConfiguration;

        public EventDeckConfiguration EventDeckConfiguration;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayersCount);
            serializer.SerializeValue(ref ChooseCharactersCount);
            serializer.SerializeValue(ref Characters);
            serializer.SerializeValue(ref MapGeneratorConfiguration);
            serializer.SerializeValue(ref DealMissionsConfiguration);
            serializer.SerializeValue(ref EventDeckConfiguration);
        }
    }
}
