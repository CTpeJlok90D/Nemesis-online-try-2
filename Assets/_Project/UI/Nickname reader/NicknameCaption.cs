using Core.Players;
using System;
using TMPro;
using UnityEngine;

namespace UI.NicknameReader
{
    public class NicknameCaption : PlayerContainerListener
    {
        [SerializeField] private TMP_Text _nicknameLabel;

        private NicknameContainer _nicknameContainer;

        protected override void OnPlayerChange(Player player)
        {
            if (_nicknameContainer != null)
            {
                _nicknameContainer.Changed -= OnNicknameChange;
            }

            if (PlayerContainer.Player == null)
            {
                _nicknameLabel.text = "</PLAYER NOT FOUND>";
                return;
            }

            _nicknameContainer = PlayerContainer.Player.GetComponent<NicknameContainer>();

            if (_nicknameContainer != null)
            {
                _nicknameContainer.Changed += OnNicknameChange;
            }

            ValidateNickname();
        }

        private void OnNicknameChange(string obj)
        {
            ValidateNickname();
        }

        private void ValidateNickname()
        {
            if (_nicknameContainer == null)
            {
                _nicknameLabel.text = "</CONTAINER NOT FOUND>";
                return;
            }

            _nicknameLabel.text = _nicknameContainer.Nickname;
        }
    }
}
