using System;
using Core.Players;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlayerTablets
{
    public class LeaveTabletButton : MonoBehaviour
    {
        [SerializeField] private PlayerTabletContainer _playerTabletContainer;

        [SerializeField] private Button _button;

        public PlayerTablet PlayerTablet => _playerTabletContainer.PlayerTablet;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
            PlayerTablet.PlayerReference.ReferenceChanged += OnPlayerChange;
            UpdateButtonView();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            PlayerTablet.PlayerReference.ReferenceChanged -= OnPlayerChange;
        }

        private void OnPlayerChange(Player previousValue, Player newValue)
        {
            UpdateButtonView();
        }

        private void OnButtonClick()
        {
            PlayerTablet.LeavePlayerTablet();
        }

        private void UpdateButtonView()
        {
            if (PlayerTablet.PlayerReference.Reference == null)
            {
                _button.gameObject.SetActive(false);
                return;
            }
            _button.gameObject.SetActive(PlayerTablet.PlayerReference.Reference.IsLocal);
        }
    }
}
