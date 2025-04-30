using System;
using System.Linq;
using Core.ActionsCards;
using Unity.Netcode;
using Unity.Netcode.Custom;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.Characters.Infection
{
    public class InfectionDeck : NetworkBehaviour
    {
        [Inject] private Config _config;

        private NetScriptableObjectList4096<ActionCard> _deck;
        
        private void Awake()
        {
            _deck = new();
        }

        private void Start()
        {
            if (IsServer)
            {
                ActionCard[] shuffledDeck = _config.InfectionCards.OrderBy(x => Random.value).ToArray();
                _deck.AddRange(shuffledDeck);
            }
        }

        [Serializable]
        public struct Config : INetworkSerializable 
        {
            public ActionCard[] InfectionCards;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref InfectionCards);
            }
        }
    }
}
