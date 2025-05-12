using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Characters.Health
{
    public class HeavyDamage : NetworkBehaviour
    {
        private NetBehaviourReference<CharacterHealth> _owner;
        public CharacterHealth Owner => _owner.Reference;

        public bool IsInitialized { get; private set; }
        
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
            
            gameObject.SetActive(true);
            NetworkObject.Spawn();
            NetworkObject.TrySetParent(owner.NetworkObject);
            return this;
        }
    }
}
