using AYellowpaper;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;

namespace UI.PlayerTablets
{
    public class PlayerTabletIsEmptyObject : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IContainsPlayerTablet> _playerTablet;
        [SerializeField] private GameObject _target;

        public PlayerTablet PlayerTablet => _playerTablet.Value.PlayerTablet;

        private void OnEnable()
        {
            PlayerTablet.PlayerReference.Changed += OnPlayerChange;
            UpdateView();
        }

        private void OnDisable()
        {
            PlayerTablet.PlayerReference.Changed -= OnPlayerChange;   
        }

        private void OnPlayerChange(NetworkObjectReference previousValue, NetworkObjectReference newValue)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _target.SetActive(PlayerTablet.PlayerReference.Reference == null);
        }
    }
}
