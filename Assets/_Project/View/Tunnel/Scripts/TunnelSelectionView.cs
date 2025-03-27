using System.Linq;
using Core.Maps;
using Core.Selection.Tunnels;
using Core.SelectionBase;
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
        private void OnEnable()
        {
            _noiseContainerSelection.SelectionStarted += OnSelectionStart;
            _noiseContainerSelection.SelectionChanged += OnSelectionChange;
            UpdateView();
        }

        private void OnDisable()
        {
            _noiseContainerSelection.SelectionChanged -= OnSelectionChange;
            _noiseContainerSelection.SelectionStarted += OnSelectionStart;
        }

        private void OnSelectionStart(ISelection sender) => UpdateView();
        private void OnSelectionChange(ISelection sender) => UpdateView();
        private void UpdateView()
        {
            _canSelectView.SetActive(_noiseContainerSelection.CanSelect(_tunnel.NoiseContainer));
            _selectedView.SetActive(_noiseContainerSelection.Contains(_tunnel.NoiseContainer));
        }
    }
}
