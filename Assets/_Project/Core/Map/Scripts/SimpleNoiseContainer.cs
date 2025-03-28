using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    public class SimpleNoiseContainer : NetworkBehaviour, INoiseContainer
    {
        public NetVariable<bool> IsNoised { get; private set; }

        IReadOnlyReactiveField<bool> INoiseContainer.IsNoised => IsNoised;

        public event IReadOnlyReactiveField<bool>.ChangedListener NoiseStatusChanged
        {
            add => IsNoised.Changed += value;
            remove => IsNoised.Changed -= value;
        }

        private void Awake()
        {
            IsNoised = new();
        }

        void INoiseContainer.Noise()
        {
            IsNoised.Value = true;
        }

        public void Clear()
        {
            IsNoised.Value = false;
        }
    }
}
