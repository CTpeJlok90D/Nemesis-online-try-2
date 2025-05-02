using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.EventsDecks
{
    [Icon("Assets/_Project/Core/Events deck/Editor/icons8-cards-deck-96.png")]
    [CreateAssetMenu(menuName = "Game/Event card")]
    public class EventCard : ScriptableObject, INetworkSerializable, IEquatable<EventCard>, INetScriptableObjectArrayElement<EventCard>
    {
        [SerializeField] private EventCardAction _action;
        [SerializeField] private AlienToken[] _aliensToMove;
        [SerializeField] private NoiseDice.Result _tunnelIndex;   
        [SerializeField] private NetScriptableObject<EventCard> _net = new();
        [SerializeField] private bool _placeCardInDiscardAfterUse = true;

        public NetScriptableObject<EventCard> Net => _net;
        public IReadOnlyCollection<AlienToken> AliensToMove => _aliensToMove;
        public NoiseDice.Result TunnelIndex => _tunnelIndex;
        public EventCardAction Action => _action;
        public bool PlaceCardInDiscardAfterUse => _placeCardInDiscardAfterUse;

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
            _aliensToMove = result._aliensToMove.ToArray();
            _tunnelIndex = result._tunnelIndex;
            _action = result._action;
        }
    }
}
