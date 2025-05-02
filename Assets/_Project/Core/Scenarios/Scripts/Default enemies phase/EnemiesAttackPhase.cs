using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.AlienAttackDecks;
using Core.Aliens;
using Core.Characters.Infection;
using Core.Entity;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scenarios.EnemiesPhase
{
    public class EnemiesAttackPhase : IChapter
    {
        private readonly PlayerTabletList _playerTabletList;
        private InfectionDeck _infectionDeck;
        
        public event IChapter.EndedListener Ended;
        
        public EnemiesAttackPhase(PlayerTabletList playerTablets)
        {
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
                List<CharacterPawn> characterPawns = enemy.RoomContent.Owner.GetContentWith<CharacterPawn>().ToList();
                
                if (characterPawns.Any() == false)
                {
                    Debug.Log($"Enemies attack phase: No players to attack. Skipping");
                    continue;
                }
                
                List<PlayerTablet> possibleTargets = characterPawns
                    .Select(c => _playerTabletList.First(x => x.CharacterPawn == c))
                    .OrderBy(x => x.OrderNumber.Value)
                    .Reverse().ToList();

                PlayerTablet target = await GetPrioritizedTarget(possibleTargets);
                enemy.Attack(target);
            }
            
            Ended?.Invoke(this);
        }

        private async UniTask<PlayerTablet> GetPrioritizedTarget(IEnumerable<PlayerTablet> possibleTargets)
        {
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

            return target;
        }
    }
}
