using System.Linq;
using Core.Maps;
using Core.Selection.Tunnels;
using TNRD;
using UnityEngine;
using Zenject;

namespace View.Tunnels
{
    public class TunnelSelectionView : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<INoiseContainer> _tunnel;
        [SerializeField] private GameObject _canSelectView;
        [SerializeField] private GameObject _selectedView;
        
        [Inject] private NoiseContainerSelection _noiseContainerSelection;
        
        private void Update()
        {
            UpdateView();
        }
        
        private void UpdateView()
        {
            _canSelectView.SetActive(_noiseContainerSelection.CanSelect(_tunnel.Value));
            _selectedView.SetActive(_noiseContainerSelection.Contains(_tunnel.Value));
        }
    }
}
