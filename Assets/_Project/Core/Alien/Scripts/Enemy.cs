using System.Collections.Generic;
using Core.Entity;
using Core.Maps;
using TNRD;
using UnityEngine;

namespace Core.Aliens
{
    [Icon("Assets/_Project/Core/Alien/Editor/monster.png")]
    [RequireComponent(typeof(RoomContent))]
    public class Enemy : NetEntity<Enemy>
    {
        [field: SerializeField] private SerializableInterface<IAlienDamageHandler> _damageHandler;
        [field: SerializeField] public AlienToken LinkedToken { get; private set; }
        [field: SerializeField] private AttackDice.Result[] _attacksToHit;
        
        public RoomContent RoomContent { get; private set; }
        
        protected override Enemy Instance
        {
            get { return this; }
        }
        
        public IReadOnlyCollection<AttackDice.Result> AttacksToHit
        {
            get { return _attacksToHit; }
        }

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
