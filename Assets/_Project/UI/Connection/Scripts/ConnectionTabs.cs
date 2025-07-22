using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.Connection
{
    public class ConnectionTabs : MonoBehaviour
    {
        [SerializeField] private GameObject _notConnectedTab;
        
        [SerializeField] private GameObject _connectedTab;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        private void OnEnable()
        {
            if (didStart)
            {
                NetworkManager.OnClientStarted += OnClientStart;
                NetworkManager.OnClientStopped += OnClientStop;
                ValidateTabs();
            }
        }

        private void Start()
        {
            NetworkManager.OnClientStarted += OnClientStart;
            NetworkManager.OnClientStopped += OnClientStop;
            ValidateTabs();
        }

        private void OnDisable()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientStarted -= OnClientStart;
                NetworkManager.OnClientStopped -= OnClientStop;
            }
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
            if (NetworkManager.IsConnectedClient) 
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
