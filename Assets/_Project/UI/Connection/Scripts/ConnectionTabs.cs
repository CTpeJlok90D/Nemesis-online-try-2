using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.Connection
{
    public class ConnectionTabs : MonoBehaviour
    {
        [SerializeField] private GameObject _notConnectedTab;
        [SerializeField] private GameObject _connectedTab;

        [Inject] private NetworkManager _networkManager;

        private void OnEnable()
        {
            ValidateTabs();
            _networkManager.OnClientStarted += OnClientStart;
            _networkManager.OnClientStopped += OnClientStop;
        }

        private void OnDisable()
        {
            _networkManager.OnClientStarted -= OnClientStart;
            _networkManager.OnClientStopped -= OnClientStop;
        }

        private void OnClientStop(bool obj) => EnableNotConnectedTab();
        private void EnableNotConnectedTab()
        {
            _connectedTab.SetActive(false);
            _notConnectedTab.SetActive(true);
        }

        private void OnClientStart() => EnableConnectedTab();
        private void EnableConnectedTab()
        {
            _connectedTab.SetActive(true);
            _notConnectedTab.SetActive(false);
        }

        private void ValidateTabs()
        {
            if (_networkManager.IsConnectedClient) 
            {
                EnableConnectedTab();
            }
            else
            {
                EnableNotConnectedTab();
            }
        }
    }
}
