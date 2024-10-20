using Core.Conncetion;
using Core.Players;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;


namespace UI.Connection._Project.UI.Connection
{
    public class JoinCodeInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        private NetworkManager _networkManager;
        private RelayConnection _relay;

        [Inject]
        public void ZInit(RelayConnection relayConnection)
        {
            _relay = relayConnection;
        }

        private void OnEnable()
        {
            if (didStart)
            {
                OnEnableAfterStart();
            }
        }


        private void Start()
        {
            _networkManager = NetworkManager.Singleton;
            OnEnableAfterStart();
        }

        private void OnEnableAfterStart()
        {
            _relay.Started += OnStart;
            _relay.Joined += OnJoin;
            Player.Left += OnPlayerLeave;
            UpdateJoinCode();
        }

        private void OnDisable()
        {
            _relay.Started -= OnStart;
            _relay.Joined -= OnJoin;
            Player.Left -= OnPlayerLeave;
        }

        private void OnStart(string joinCode)
        {
            UpdateJoinCode();
        }

        private void OnJoin()
        {
            _inputField.readOnly = true;
        }

        private void UpdateJoinCode()
        {
            _inputField.text = _relay.JoinCode;
        }

        private void OnPlayerLeave(Player player)
        {
            if (player.IsLocalPlayer)
                _inputField.readOnly = false;
        }
    }
}