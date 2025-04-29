using System;
using System.Linq;
using Core.Aliens;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class AlienCardsHealthHandler : NetworkBehaviour, IAlienDamageHandler
    {
        [field: SerializeField] private int CardCount { get; set; } = 1;
        [field: SerializeField] private NetVariable<int> Damage { get; set; }

        [Inject] private AlienAttackDeck _alienAttackDeck;
        
        private void Awake()
        {
            Damage = new();
        }

        public void Handle(Enemy target, int damage, bool disableKilling = false)
        {
            Damage.Value += damage;
            
            if (disableKilling)
            {
                return;
            }
            TryKill();
        }

        public bool TryKill()
        {
            AlienAttackCard[] cards = _alienAttackDeck.Pick(CardCount);
            int sumEndurance = cards.Sum(x => x.Endurance);

            if (Damage.Value >= sumEndurance)
            {
                NetworkObject.Despawn(gameObject);
                return true;
            }

            return false;
        }
    }
}
