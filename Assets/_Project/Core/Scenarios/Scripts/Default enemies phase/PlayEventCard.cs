using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.AliensBags;
using Core.Entities;
using Core.EventsDecks;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.EnemiesPhase
{
    public class PlayEventCard : IChapter
    {
        private EventsDeck _eventDeck;
        private AliensBag _alienBag;
        private DiContainer _diContainer;
        
        public PlayEventCard(EventsDeck eventsDeck, AliensBag alienBag, DiContainer diContainer)
        {
            _eventDeck = eventsDeck;
            _alienBag = alienBag;
            _diContainer = diContainer;
        }
        
        public void Begin()
        {
            _ = Play();
        }

        private async UniTask Play()
        {
            EventCard card = await _eventDeck.PickOne();
            
            MoveAliens(card);

            if (card.Action == null)
            {
                Debug.LogWarning($"{card} have no event action. Skipping phase", card);
                Ended?.Invoke(this);
                return;
            }
            
            _diContainer.Inject(card.Action);
            
            Debug.Log($"Event card {card} was played");
            card.Action.Execute();
            
            Ended?.Invoke(this);
        }

        private void MoveAliens(EventCard card)
        {
            foreach (Enemy enemy in NetEntity<Enemy>.Instances)
            {
                if (card.AliensToMove.Contains(enemy.LinkedToken.Value) == false)
                {
                    continue;
                }
                
                
                RoomCell room = enemy.RoomContent.Owner;
                IReadOnlyCollection<RoomContent> otherContent = room.RoomContents;
                if (otherContent.Any(x => x.TryGetComponent(out CharacterPawn pawn)))
                {
                    continue;
                }
                
                INoiseContainer noiseContainer = room.GetTunnelForNoiseRollResult(card.TunnelIndex);

                if (noiseContainer is Tunnel tunnel)
                {
                    RoomCell roomCell = tunnel.RoomCells.First(x => x != room);
                    roomCell.AddContent(enemy.RoomContent);
                    Debug.Log($"{enemy} was moved in {room}");
                    return;
                }
                
                Debug.Log($"{enemy} left the map");
                _alienBag.Add(enemy.LinkedToken.Value);
                enemy.NetworkObject.Despawn();
            }
        }

        public event IChapter.EndedListener Ended;
    }
}
