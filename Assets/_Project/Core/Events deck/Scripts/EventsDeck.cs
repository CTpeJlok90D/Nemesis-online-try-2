using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core.EventsDecks
{
    [Icon("Assets/_Project/Core/Events deck/Editor/icons8-cards-deck-96.png")]
    [RequireComponent(typeof(NetworkObject))]
    public class EventsDeck : NetworkBehaviour
    {
        private NetScriptableObjectList4096<EventCard> _deck;
        private NetScriptableObjectList4096<EventCard> _discard;
        
        [Inject]
        public void Initialize(EventDeckConfiguration configuration)
        {
            _deck = new();
            _deck.AddRange(configuration.Deck);
            
            _discard = new();
        }

        public async UniTask<EventCard> PickOne()
        {
            EventCard[] eventCards = await _deck.GetElements();

            if (eventCards.Length == 0)
            {
                await Shuffle();
                eventCards = await _deck.GetElements();
            }
            
            EventCard result = eventCards.First();
            return result;
        }

        public async UniTask Shuffle()
        {
            EventCard[] discardCards = await _discard.GetElements();
            EventCard[] deckCards = await _deck.GetElements();
            
            List<EventCard> deck = new();
            deck.AddRange(deckCards);
            deck.AddRange(discardCards);
            deck = deck.OrderBy(x => Random.value).ToList();
            
            _discard.Clear();
            _deck.SetElements(deck);
        }
    }
}