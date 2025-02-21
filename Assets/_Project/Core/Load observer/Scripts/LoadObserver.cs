using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.LoadObservers
{
    [RequireComponent(typeof(NetworkObject))]
    public class LoadObserver : NetworkBehaviour
    {
        public delegate void StatusChangedListener(ulong clientId, Status oldStatus, Status newStatus);

        private SerializedDictionary<ulong, Status> _loadStatuses = new();

        private NetworkManager _networkManager;

        private NetworkVariable<bool> _everyOneIsReady;

        public event StatusChangedListener StatusChanged;

        public IReadOnlyDictionary<ulong, Status> LoadStatuses => _loadStatuses;

        public bool EveryoneIsReady => _everyOneIsReady.Value;

        public LoadObserver Instantiate(NetworkManager networkManager)
        {
            gameObject.SetActive(false);
            LoadObserver result = Instantiate(this);
            gameObject.SetActive(true);

            result._networkManager = networkManager;
            result.gameObject.SetActive(true);

            return result;
        }

        public Status GetClientStatus(ulong clientID)
        {
            if (EveryoneIsReady)
            {
                return Status.Ready;
            }

            if (_loadStatuses.TryGetValue(clientID, out Status result))
            {
                return result;
            }

            _loadStatuses.Add(clientID, Status.Ready);
            StatusChanged?.Invoke(clientID, Status.Ready, Status.Ready);

            return Status.Ready;
        }

        private void Awake()
        {
            _everyOneIsReady = new(true);
            _networkManager.OnClientStarted += OnClientStart;
            _networkManager.OnClientStopped += OnClientStop;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _networkManager.OnClientStarted -= OnClientStart;
            _networkManager.OnClientStopped -= OnClientStop;
        }

        private void OnClientStart()
        {
            _networkManager.SceneManager.OnLoad += OnLoadStart;
            _networkManager.SceneManager.OnLoadComplete += OnLoadComplete;
        }

        private void OnClientStop(bool obj)
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoad -= OnLoadStart;
                _networkManager.SceneManager.OnLoadComplete -= OnLoadComplete;
            }
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (NetworkManager.IsServer == false)
            {
                return;
            }

            ValidateClients();
            Status oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = Status.Ready;
            _everyOneIsReady.Value = _loadStatuses.Values.All(x => x is Status.Ready);
            StatusChanged_RPC(clientId, oldStatus, _loadStatuses[clientId]);
        }

        private void OnLoadStart(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            if (NetworkManager.IsServer == false)
            {
                return;
            }

            ValidateClients();
            Status oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = Status.NotReady;
            _everyOneIsReady.Value = _loadStatuses.Values.All(x => x is Status.Ready);
            StatusChanged_RPC(clientId, oldStatus, _loadStatuses[clientId]);
        }

        [Rpc(SendTo.Everyone)]
        private void StatusChanged_RPC(ulong clientId, Status oldStatus, Status newStatus)
        {
            if (_loadStatuses.ContainsKey(clientId) == false)
            {
                _loadStatuses.Add(clientId, newStatus);
            }

            StatusChanged?.Invoke(clientId, oldStatus, _loadStatuses[clientId]);
        }

        private void ValidateClients()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (_loadStatuses.ContainsKey(clientId) == false)
                {
                    _loadStatuses.Add(clientId, Status.NotReady);
                }
            }
        }

            public enum Status
        {
            Ready,
            NotReady
        }
    }
}