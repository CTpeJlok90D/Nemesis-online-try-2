using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AliensTablets
{
    [Icon("Assets/_Project/Core/Aliens tablet/Editor/icons8-weakness-cards-96.png")]
    [CreateAssetMenu(menuName = "Game/Aliens/Alient weakness card")]
    public class AlienWeaknessCard : ScriptableObject, INetworkSerializable, IEquatable<AlienWeaknessCard>
    {
        [field: SerializeField] private AlienWeakness _alienWeakness;
        [field: SerializeField] private NetScriptableObject<AlienWeaknessCard> _net = new();
        public AssetReferenceT<AlienWeaknessCard> LoadAssetReference => _net.SelfAssetReference;

        private bool _isActive;

        public AlienWeaknessCard Instantiate()
        {
            AlienWeaknessCard instance = Instantiate(this);
            instance._isActive = _isActive;
            instance._alienWeakness = Instantiate(instance._alienWeakness);
            instance._alienWeakness.IsActive = instance._isActive;
            return instance;
        }

        public bool Equals(AlienWeaknessCard other)
        {
            return _net.SelfAssetReference.RuntimeKey == other._net.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.OnNetworkSerialize(serializer, this);
            serializer.SerializeValue(ref _isActive);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _net.OnValidate(this);
        }
#endif
    }
}
