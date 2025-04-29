using System.Collections.Generic;
using Core.Maps;
using TNRD;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.Aliens
{
    [Icon("Assets/_Project/Core/Alien/Editor/monster.png")]
    [RequireComponent(typeof(RoomContent))]
    public class Enemy : NetworkBehaviour
    {
        [SerializeField] private SerializableInterface<IAlienDamageHandler> _damageHandler;
        [SerializeField] private AttackDice.Result[] _attacksToHit;
        public RoomContent RoomContent { get; private set; }
        
        public AttackDice.Result[] AttacksToHit => _attacksToHit;

        public Enemy Instantiate()
        {
            gameObject.SetActive(false);
            Enemy enemy = Instantiate(this);
            gameObject.SetActive(true);

            enemy.gameObject.SetActive(true);
            enemy.NetworkObject.Spawn();
            return enemy;
        }

        private void Awake()
        {
            RoomContent = GetComponent<RoomContent>();
        }

        public void Damage(int value = 1)
        {
            _damageHandler.Value.Handle(this, value);
        }
    }
}
