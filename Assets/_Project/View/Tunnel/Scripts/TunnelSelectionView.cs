using System.Linq;
using Core.Maps;
using Core.Selection.Tunnels;
using UnityEngine;
using Zenject;

namespace View.Tunnels
{
    public class TunnelSelectionView : MonoBehaviour
    {
        [SerializeField] private Tunnel _tunnel;
        [SerializeField] private GameObject _canSelectView;
        [SerializeField] private GameObject _selectedView;
        
        [Inject] private NoiseContainerSelection _noiseContainerSelection;
        
        private void Update()
        {
            UpdateView();
        }
        
        private void UpdateView()
        {
            _canSelectView.SetActive(_noiseContainerSelection.CanSelect(_tunnel.NoiseContainer));
            _selectedView.SetActive(_noiseContainerSelection.Contains(_tunnel.NoiseContainer));
        }
    }
}
