using System;
using Core.Players;
using Core.PlayerTablets;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace UI.PlayerTablets
{
    public class PlayerNickname : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        [SerializeField] private PlayerTabletContainer _playerTabletContainer;

        private NicknameContainer _lastNicknameContainer;

        private Player _lastPlayer;

        private void OnEnable()
        {
            _playerTabletContainer.PlayerTablet.PlayerReference.Changed += OnPlayerChange;
            UpdatePlayerContainer();
            UpdateLabel();
        }

        private void OnDisable()
        {
            _playerTabletContainer.PlayerTablet.PlayerReference.Changed -= OnPlayerChange;
        }

        private void OnPlayerChange(NetworkObjectReference previousValue, NetworkObjectReference newValue)
        {
            UpdatePlayerContainer();
            UpdateLabel();
        }

        private void UpdatePlayerContainer()
        {
            if (_lastNicknameContainer != null)
            {
                _lastNicknameContainer.Changed -= OnNicknameChange;
            }

            _lastNicknameContainer = null;

            _lastPlayer = _playerTabletContainer.PlayerTablet.PlayerReference.Reference;
            if (_lastPlayer == null)
            {
                return;
            }

            if (_lastPlayer.TryGetComponent(out NicknameContainer nicknameContainer))
            {
                _lastNicknameContainer = nicknameContainer;
                _lastNicknameContainer.Changed += OnNicknameChange;
                return;
            }
        }

        private void OnNicknameChange(string obj)
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (_lastNicknameContainer == null)
            {
                _label.text = "EMPTY";
                return;
            }

            _label.text = _lastNicknameContainer.Value;
            if (_lastPlayer.IsLocal)
            {
                _label.fontStyle = FontStyles.Underline;
            }
            else
            {
                _label.fontStyle = FontStyles.Normal;
            }
        }
    }
}
