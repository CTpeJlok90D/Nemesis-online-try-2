using System.Linq;
using Core.Aliens;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.AlienAttackDecks
{
    public interface IAlienAttack
    {
        public void Attack(Enemy executor, PlayerTablet target);
    }

    public static class EnemyAttack
    {
        public static void Attack(this Enemy executor, PlayerTablet target)
        {
            if (executor.TryGetComponent(out IAlienAttackOverride alienAttackOverride))
            {
                alienAttackOverride.Attack(executor, target);
                return;
            }
            
            AlienAttackCard card = Game.AlienAttackDeck.PickOne();
            if (card.PossibleAttackers.Contains(executor.LinkedToken.Value))
            {
                card.AlienAttack.Attack(executor, target);
                Debug.Log($"{executor} is attacking {target} by card: {card}");
            }
            else
            {
                Debug.Log($"{executor} is not attacking {target} by card: {card}");
            }
        }
    }
}
