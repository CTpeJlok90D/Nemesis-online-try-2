using System.Linq;
using Core.Maps;
using Core.Selection.Tunnels;
using TNRD;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace View.Tunnels
{
    [RequireComponent(typeof(PointerEvents))]
    public class TunnelToggleButton : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<INoiseContainer> _noiseContainer;
        [Inject] private NoiseContainerSelection _noiseContainerSelection;
        
        private PointerEvents _pointerEvents;

        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnPointerClicked;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnPointerClicked;
        }

        private void OnPointerClicked(PointerEvents sender, PointerEventData eventdata)
        {
            if (_noiseContainerSelection.Contains(_noiseContainer.Value))
            {
                _noiseContainerSelection.Remove(_noiseContainer.Value);
                return;
            }

            if (_noiseContainerSelection.CanSelect(_noiseContainer.Value))
            {
                _noiseContainerSelection.Add(_noiseContainer.Value);
            }
        }
    }
}
