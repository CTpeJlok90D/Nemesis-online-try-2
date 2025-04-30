using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core.EventsDeck
{
    [Icon("Assets/_Project/Core/Events deck/Editor/icons8-cards-deck-96.png")]
    [RequireComponent(typeof(NetworkObject))]
    public class EventsDeck : NetworkBehaviour
    {
        public delegate void EventCardPlayedListener(EventsDeck sender, EventCard card);
        
        [SerializeField] private List<EventCard> _deck;

        [SerializeField] private List<EventCard> _discard;

        public EventCardPlayedListener EventCardPlayed;

        [Inject]
        public void Initialize(EventDeckConfiguration configuration)
        {
            _deck = configuration.Deck.ToList();
            _discard = new();
        }
    }
}