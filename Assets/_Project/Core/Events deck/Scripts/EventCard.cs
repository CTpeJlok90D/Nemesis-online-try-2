using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.EventsDeck
{
    [Icon("Assets/_Project/Core/Events deck/Editor/icons8-cards-deck-96.png")]
    [CreateAssetMenu(menuName = "Game/Event card")]
    public class EventCard : ScriptableObject, INetworkSerializable, IEquatable<EventCard>, INetScriptableObjectArrayElement<EventCard>
    {
        [SerializeField] private EventCardAction _action;

        [SerializeField] private AlienToken[] _aliensTokens;

        [SerializeField] private int _corridorIndex = 1;   
        
        [SerializeField] private NetScriptableObject<EventCard> _net = new();

        public NetScriptableObject<EventCard> Net => _net;

        public IReadOnlyCollection<AlienToken> AlienTokens => _aliensTokens;

        public int CorridorIndex => _corridorIndex;

        public EventCardAction Action => _action;

        public bool Equals(EventCard other)
        {
            return _net.RuntimeLoadKey == other._net.RuntimeLoadKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.OnNetworkSerialize(serializer, this);
            _net.Preloaded += OnLoad; 
        }

        private void OnLoad(EventCard result)
        {
            _net.Preloaded -= OnLoad;
            _aliensTokens = result._aliensTokens.ToArray();
            _corridorIndex = result._corridorIndex;
            _action = result._action;
        }
    }
}
