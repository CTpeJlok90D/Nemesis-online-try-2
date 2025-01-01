using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    public class SimpleNoiseContainer : NetworkBehaviour, INoiseContainer
    {
        public NetVariable<bool> IsNoised { get; private set; }

        bool INoiseContainer.IsNoised => IsNoised.Value;

        public event NetVariable<bool>.OnValueChangedDelegate NoiseStatusChanged
        {
            add => IsNoised.Changed += value;
            remove => IsNoised.Changed -= value;
        }

        private void Awake()
        {
            IsNoised = new();
        }

        public void Noise()
        {
            IsNoised.Value = true;
        }

        public void Clear()
        {
            IsNoised.Value = false;
        }
    }
}
