using System.Linq;
using Core.Maps;
using Core.Selection.Tunnels;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace View.Tunnels
{
    [RequireComponent(typeof(PointerEvents))]
    public class TunnelToggleButton : MonoBehaviour
    {
        [SerializeField] private Tunnel _tunnel;
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
            if (_noiseContainerSelection.Contains(_tunnel.NoiseContainer))
            {
                _noiseContainerSelection.Remove(_tunnel.NoiseContainer);
                return;
            }

            if (_noiseContainerSelection.CanSelect(_tunnel.NoiseContainer))
            {
                _noiseContainerSelection.Add(_tunnel.NoiseContainer);
            }
        }
    }
}
