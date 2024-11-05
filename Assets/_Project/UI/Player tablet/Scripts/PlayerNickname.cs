using System;
using Core.Lobbies;
using Core.Players;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.PlayerTablets
{
    public class PlayerNickname : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private PlayerTabletContainer _playerTabletContainer;

        private void OnEnable()
        {
            UpdateLabel();
            _playerTabletContainer.PlayerTablet.PlayerReference.Changed += OnPlayerChange;
        }

        private void OnDisable()
        {
            _playerTabletContainer.PlayerTablet.PlayerReference.Changed -= OnPlayerChange;
        }

        private void OnPlayerChange(NetworkObjectReference previousValue, NetworkObjectReference newValue)
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            Player player = _playerTabletContainer.PlayerTablet.PlayerReference.Reference;
            if (player == null)
            {
                _label.text = "ERROR";
                return;
            }

            if (player.TryGetComponent(out NicknameContainer nicknameContainer))
            {
                _label.text = nicknameContainer.Value;
                return;
            }
            _label.text = "ERROR";
        }
    }
}
