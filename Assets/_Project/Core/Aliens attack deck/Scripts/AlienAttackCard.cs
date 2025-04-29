using System;
using TNRD;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.AlienAttackDecks
{
    [CreateAssetMenu(menuName = "Game/Alien attack card")]
    public class AlienAttackCard : ScriptableObject, INetworkSerializable, IEquatable<AlienAttackCard>, INetScriptableObjectArrayElement<AlienAttackCard>
    {
        [field: SerializeField] public SerializableInterface<IAlienAttack> _attackAction = new();
        [field: SerializeField] public int Endurance = 3;
        [field: SerializeField] private NetScriptableObject<AlienAttackCard> _attackCard = new();
        public NetScriptableObject<AlienAttackCard> Net => _attackCard;

        public IAlienAttack AlienAttack => _attackAction.Value;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _attackCard.Preloaded += OnPreLoad;
            _attackCard.OnNetworkSerialize(serializer, this);
        }

        private void OnPreLoad(AlienAttackCard result)
        {
            _attackCard.Preloaded -= OnPreLoad;
            Endurance = result.Endurance;
            _attackAction.Value = result._attackAction.Value;
        }

        public bool Equals(AlienAttackCard other)
        {
            return Net.RuntimeLoadKey == other.Net.RuntimeLoadKey;
        }
    }
}
