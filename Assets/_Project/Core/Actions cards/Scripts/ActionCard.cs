using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using Unity.Collections;
using UnityEngine;


namespace Core.ActionsCards
{
    [Icon("Assets/_Project/Core/Actions cards/Editor/icons8-empty-cards-96.png")]
    [CreateAssetMenu(menuName = "Game/Action cards/Card")]
    public class ActionCard : ScriptableObject, INetworkSerializable, IEquatable<ActionCard>, INetScriptableObjectArrayElement<ActionCard>
    {
        [field: SerializeField] public int Cost = 0;
        [field: SerializeField] public ActionCardEffect Effect { get; private set; }
        [field: SerializeField] private NetScriptableObject<ActionCard> _actionCard = new();
        [field: SerializeField] public string ID { get; private set; }

        public NetScriptableObject<ActionCard> Net => _actionCard;

        public bool Equals(ActionCard other)
        {
            return _actionCard.SelfAssetReference.RuntimeKey == other._actionCard.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _actionCard.Preloaded += OnCardLoaded;
            _actionCard.OnNetworkSerialize(serializer, this);
        }

        private void OnCardLoaded(ActionCard result)
        {
            _actionCard.Preloaded -= OnCardLoaded;
            Effect = result.Effect;
            Cost = result.Cost;
            ID = result.ID;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ID = name;
        }
#endif
    }
}
