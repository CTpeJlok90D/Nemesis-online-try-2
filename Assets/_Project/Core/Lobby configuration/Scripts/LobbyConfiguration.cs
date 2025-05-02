using System;
using Core.ActionsCards;
using Core.AlienAttackDecks;
using Core.Characters.Health;
using Core.CharacterInventories;
using Core.Characters;
using Core.Characters.Infection;
using Core.EventsDecks;
using Core.Maps;
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

        public ActionCardsDeck.Config ActionCardsDeckConfiguration;
        
        public Character[] Characters;

        public MapGeneratorConfiguration MapGeneratorConfiguration;

        public MissionsDealerConfiguration DealMissionsConfiguration;

        public EventDeckConfiguration EventDeckConfiguration;
        
        public EnemiesConfig EnemiesConfig;

        public AlienAttackDeckConfig AliensAttackDeckConfig;
        
        public KitStartConfig KitStartConfig;
        
        public HeavyDamageDeck.Config HeavyDamageDeckConfig;
        
        public InfectionDeck.Config InfectionDeckConfiguration;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayersCount);
            serializer.SerializeValue(ref ChooseCharactersCount);
            serializer.SerializeValue(ref Characters);
            serializer.SerializeValue(ref MapGeneratorConfiguration);
            serializer.SerializeValue(ref DealMissionsConfiguration);
            serializer.SerializeValue(ref EventDeckConfiguration);
            serializer.SerializeValue(ref ActionCardsDeckConfiguration);
            serializer.SerializeValue(ref EnemiesConfig);
            serializer.SerializeValue(ref AliensAttackDeckConfig);  
            serializer.SerializeValue(ref KitStartConfig);
            serializer.SerializeValue(ref HeavyDamageDeckConfig);
            serializer.SerializeValue(ref InfectionDeckConfiguration);
        }
    }
}
