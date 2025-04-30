using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.AlienAttackDecks;
using Core.Aliens;
using Core.Entity;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scenarios.EnemiesPhase
{
    public class EnemiesAttackPhase : IChapter
    {
        private AlienAttackDeck _alienAttackDeck;
        
        private PlayerTabletList _playerTabletList;
        
        public event IChapter.EndedListener Ended;
        
        public EnemiesAttackPhase(AlienAttackDeck deck, PlayerTabletList playerTablets)
        {
            _alienAttackDeck = deck;
            _playerTabletList = playerTablets;
        }
        
        public void Begin()
        {
            _ = MakeDamage();
        }

        private async UniTask MakeDamage()
        {
            IReadOnlyCollection<Enemy> enemies = NetEntity<Enemy>.Instances;

            foreach (Enemy enemy in enemies)
            {
                List<PlayerTablet> possibleTargets = new(); 
                
                foreach (RoomContent roomContent in enemy.RoomContent.Owner)
                {
                    if (roomContent.TryGetComponent(out CharacterPawn characterPawn))
                    {
                        PlayerTablet characterOwner = _playerTabletList.First(x => x.CharacterPawn ==  characterPawn);
                        possibleTargets.Add(characterOwner);
                    }
                }

                if (possibleTargets.Any() == false)
                {
                    continue;
                }

                PlayerTablet target = possibleTargets.First();
                IReadOnlyCollection<ActionCard> hand = await target.CharacterPawn.ActionCardsDeck.GetHand();
                int handSize = hand.Count;
                foreach (PlayerTablet i in possibleTargets)
                {
                    hand = await i.CharacterPawn.ActionCardsDeck.GetHand();
                    if (hand.Count < handSize)
                    {
                        target = i;
                        handSize = hand.Count;
                    }
                }

                AlienAttackCard card = _alienAttackDeck.PickOne();
                
                if (card.PossibleAttackers.Contains(enemy.LinkedToken))
                {
                    card.AlienAttack.Attack(enemy, target);
                    Debug.Log($"{enemy} is attacking {target} by card: {card}");
                }
                else
                {
                    Debug.Log($"{enemy} is not attacking {target} by card: {card}");
                }
                
                Ended?.Invoke(this);
            }
        }
    }
}
