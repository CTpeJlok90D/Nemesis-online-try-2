using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Aliens
{
    [CreateAssetMenu(menuName = "Game/Aliens/Token")]
    [Icon("Assets/_Project/Core/Alien/Editor/token-adult.png")]
    public class AlienToken : ScriptableObject, INetworkSerializable, IEquatable<AlienToken>
    {
        [field: SerializeField] private NetScriptableObject<AlienToken> _self = new();
        
        [field: SerializeField] public string Id { get; private set; }

        [field: SerializeField] public int AttackReaction { get; private set; }

        public string LoadKey => _self.SelfAssetReference.RuntimeKey.ToString();

        public bool Equals(AlienToken other)
        {
            return 
                _self.SelfAssetReference.RuntimeKey == other._self.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _self.Loaded += OnLoad;
            _self.OnNetworkSerialize(serializer, this);
        }

        private void OnLoad(AlienToken result)
        {
            _self.Loaded -= OnLoad;
            Id = result.Id;
            AttackReaction = result.AttackReaction;
        }
    }
}
