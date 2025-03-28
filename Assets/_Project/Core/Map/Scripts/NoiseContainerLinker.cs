using TNRD;
using Unity.Netcode;
using UnityEngine;

namespace Core.Maps
{
    public class NoiseContainerLinker : MonoBehaviour, INoiseContainer
    {
        [SerializeField] private SerializableInterface<INoiseContainer> _linkedContainer;
        public IReadOnlyReactiveField<bool> IsNoised => _linkedContainer.Value.IsNoised;

        public void Clear()
        {
            _linkedContainer.Value.Clear();
        }

        public NetworkObject NetworkObject => _linkedContainer.Value.NetworkObject;

        void INoiseContainer.Noise()
        {
            _linkedContainer.Value.Noise();
        }
    }
}
