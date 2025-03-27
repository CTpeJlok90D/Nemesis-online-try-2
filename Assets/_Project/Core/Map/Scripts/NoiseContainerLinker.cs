using Unity.Netcode;
using UnityEngine;

namespace Core.Maps
{
    public class NoiseContainerLinker : MonoBehaviour, INoiseContainer
    {
        [SerializeField] private SimpleNoiseContainer _linkedContainer;
        public IReadOnlyReactiveField<bool> IsNoised => _linkedContainer.IsNoised;

        public void Clear()
        {
            _linkedContainer.Clear();
        }

        public NetworkObject NetworkObject => _linkedContainer.NetworkObject;

        public void Noise()
        {
            _linkedContainer.Noise();
        }
    }
}
