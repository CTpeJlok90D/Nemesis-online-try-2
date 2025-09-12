using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Characters.Health
{
    public class HeavyDamage : NetworkBehaviour
    {
        [SerializeField] private HeavyDamageAction _linkedAction;
        [SerializeField] private string _id;
        private NetBehaviourReference<CharacterHealth> _owner;
        private NetVariable<bool> _isTreated;


        public CharacterHealth Owner => _owner.Value;
        public string ID => _id;
        public bool IsInitialized { get; private set; }
        public IReadOnlyReactiveField<bool> IsTreated => _isTreated;
        

        private void Awake()
        {
            if (IsInitialized == false && NetworkManager.IsServer)
            {
                Debug.LogError($"use HeavyDamage.Instantiate methode to create instance of this object or HeavyDamage.Init to initialize it");
                Destroy(gameObject);
            }
        }

        public HeavyDamage Instantiate(CharacterHealth owner)
        {
            gameObject.SetActive(false);
            HeavyDamage result = Instantiate(this);
            gameObject.SetActive(true);

            return result.Init(owner);
        }

        public HeavyDamage Init(CharacterHealth owner)
        {
            IsInitialized = true;
            _owner = new(owner);
            _isTreated = new(false);

            gameObject.SetActive(true);
            NetworkObject.Spawn();
            NetworkObject.TrySetParent(owner.NetworkObject);
            return this;
        }

        public void Tread()
        {
            _isTreated.Value = true;
        }
    }
}
