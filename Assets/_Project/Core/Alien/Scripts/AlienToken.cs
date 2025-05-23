using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Aliens
{
    [CreateAssetMenu(menuName = "Game/Aliens/Token")]
    [Icon("Assets/_Project/Core/Alien/Editor/token-adult.png")]
    public class AlienToken : ScriptableObject, INetworkSerializable, IEquatable<AlienToken>, INetScriptableObjectArrayElement<AlienToken>
    {
        [field: SerializeField] private NetScriptableObject<AlienToken> _self = new();
        
        [field: SerializeField] public string AlienType { get; private set; }

        [field: SerializeField] public int AttackReaction { get; private set; }
        
        [field: SerializeField] public bool IsEmpty { get; private set; }

        public string LoadKey => _self.SelfAssetReference.RuntimeKey.ToString();

        public NetScriptableObject<AlienToken> Net => _self;

        public bool Equals(AlienToken other)
        {
            return 
                _self.SelfAssetReference.RuntimeKey == other._self.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _self.Preloaded += OnLoad;
            _self.OnNetworkSerialize(serializer, this);
        }

        private void OnLoad(AlienToken result)
        {
            _self.Preloaded -= OnLoad;
            AlienType = result.AlienType;
            IsEmpty = result.IsEmpty;
            AttackReaction = result.AttackReaction;
        }
    }
}
