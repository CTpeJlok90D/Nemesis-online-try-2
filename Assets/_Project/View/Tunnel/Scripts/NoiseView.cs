using Core;
using Core.Maps;
using TNRD;
using UnityEngine;

namespace View.Tunnels
{
    public class NoiseView : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<INoiseContainer> _noiseContainer;
        [SerializeField] private GameObject _target;
        public IReadOnlyReactiveField<bool> IsNoised => _noiseContainer.Value.IsNoised;
        
        private void OnEnable()
        {
            IsNoised.Changed += OnInNoisedChange;
        }

        private void OnDisable()
        {
            IsNoised.Changed -= OnInNoisedChange;
        }

        private void OnInNoisedChange(bool oldvalue, bool newvalue)
        {
            _target.SetActive(newvalue);
        }
    }
}