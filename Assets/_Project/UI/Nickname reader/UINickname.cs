using System;
using Core.Players;
using TMPro;
using UnityEngine;

namespace UI.Nickname_reader
{
    public class UINickname : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        private NicknameContainer _localNicknameContainer;
        private const string NICKNAME_SAVE_KEY = "NICKNAME";

        public event Action<string> NicknameChanged;

        private void Start()
        {
            if (PlayerPrefs.HasKey(NICKNAME_SAVE_KEY))
                _inputField.text = PlayerPrefs.GetString(NICKNAME_SAVE_KEY);

            Player.LocalPlayerSpawned += OnLocalPlayerSpawn;
            _inputField.onValueChanged.AddListener(ValueChanged);
        }

        private void OnDestroy()
        {
            Player.LocalPlayerSpawned -= OnLocalPlayerSpawn;
            _inputField.onValueChanged.RemoveListener(ValueChanged);
        }

        private void ValueChanged(string newValue) => NicknameChanged?.Invoke(newValue);

        private void OnLocalPlayerSpawn(Player player)
        {
            _localNicknameContainer = player.GetComponent<NicknameContainer>();
            UpdateNick(_inputField.text);
        }

        private void UpdateNick(string newNick)
        {
            PlayerPrefs.SetString(NICKNAME_SAVE_KEY, newNick);

            _localNicknameContainer.Nickname = newNick;
        }
    }
}
