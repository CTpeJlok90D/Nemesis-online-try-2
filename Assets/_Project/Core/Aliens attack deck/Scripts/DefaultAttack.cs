using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Aliens;
using Core.Characters.Health;
using Core.Characters.Tokens;
using Core.PlayerTablets;
using UnityEngine;
using static Core.Characters.Infection.Game;

namespace Core.AlienAttackDecks
{
    [CreateAssetMenu(menuName = "Game/Aliens/Attack")]
    public class DefaultAttack : ScriptableObject, IAlienAttack
    {
        [field: SerializeField] public int LightDamage { get;  private set; }
        [field: SerializeField] public int HeavyDamage { get;  private set; }
        [field: SerializeField] public int HeavyDamageCountToDeath { get; private set; } = 10;
        [field: SerializeField] public int InfectionCards { get; private set; }
        [field: SerializeField] public CharacterToken[] _tokens;
        
        public IReadOnlyCollection<CharacterToken> Tokens => _tokens;
        
        public void Attack(Enemy executor, PlayerTablet target)
        {
            CharacterHealth health = target.Health;
            
            if (health.HeavyDamages.Count() >= HeavyDamageCountToDeath)
            {
                health.ForceKill();
                return;
            }
            
            health.LightDamage(LightDamage);
            health.HeavyDamage(HeavyDamage);
            int infectionCount = InfectionCards;
            while (infectionCount > 0)
            {
                ActionCard actionCard = InfectionDeck.PickOne();
                target.ActionCardsDeck.AddCardToDiscard(actionCard);
                infectionCount--;
            }
            target.CharacterPawn.Tokens.AddRange(Tokens);
        }
    }
}
