using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.ActionsCards
{
    [Icon("Assets/_Project/Core/Player actions/Editor/icons8-empty-cards-96.png")]
    [CreateAssetMenu(menuName = "Game/Action cards/Card")]
    public class ActionCard : ScriptableObject, INetworkSerializable, IEquatable<ActionCard>, INetScriptableObjectArrayElement<ActionCard>
    {
        [field: SerializeField] public int Cost = 0;
        [field: SerializeField] private NetScriptableObject<ActionCard> _actionCard = new();
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public InfectionType Type { get; private set; }

        public NetScriptableObject<ActionCard> Net => _actionCard;

        public bool Equals(ActionCard other)
        {
            return Net.RuntimeLoadKey == other.Net.RuntimeLoadKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _actionCard.Preloaded += OnCardLoaded;
            _actionCard.OnNetworkSerialize(serializer, this);
        }

        private void OnCardLoaded(ActionCard result)
        {
            _actionCard.Preloaded -= OnCardLoaded;
            Cost = result.Cost;
            ID = result.ID;
        }

        public enum InfectionType
        {
            Basic,
            FakeInfection,
            Infection,
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ID = name;
        }
#endif
    }
}
