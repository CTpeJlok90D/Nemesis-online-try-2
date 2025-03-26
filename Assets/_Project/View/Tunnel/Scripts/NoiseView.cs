using Core;
using Core.Maps;
using TNRD;
using UnityEngine;

namespace View.Tunnels
{
    [DefaultExecutionOrder(1)]
    public class NoiseView : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<INoiseContainer> _noiseContainer;
        [SerializeField] private GameObject _target;
        public IReadOnlyReactiveField<bool> IsNoised => _noiseContainer.Value.IsNoised;
        
        private void OnEnable()
        {
            IsNoised.Changed += OnInNoisedChange;
            UpdateView();
        }

        private void Start()
        {
            UpdateView();
        }

        private void OnDisable()
        {
            IsNoised.Changed -= OnInNoisedChange;
        }

        private void OnInNoisedChange(bool oldvalue, bool newvalue)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _target.SetActive(IsNoised.Value);
        }
    }
}