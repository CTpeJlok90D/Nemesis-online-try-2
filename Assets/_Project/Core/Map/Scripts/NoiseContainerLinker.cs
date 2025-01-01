using UnityEngine;

namespace Core.Maps
{
    public class NoiseContainerLinker : MonoBehaviour, INoiseContainer
    {
        [SerializeField] private SimpleNoiseContainer _linkedContainer;
        public bool IsNoised => _linkedContainer.IsNoised.Value;

        public void Clear()
        {
            _linkedContainer.Clear();
        }

        public void Noise()
        {
            _linkedContainer.Noise();
        }
    }
}
