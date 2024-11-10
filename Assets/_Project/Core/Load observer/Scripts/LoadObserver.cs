using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.LoadObservers
{
    public class LoadObserver : MonoBehaviour
    {
        public delegate void StatusChangedListener(ulong clientId, bool oldStatus, bool newStatus);

        private SerializedDictionary<ulong, bool> _loadStatuses = new();

        private NetworkManager _networkManager;

        public event StatusChangedListener StatusChanged;

        public IReadOnlyDictionary<ulong, bool> LoadStatuses => _loadStatuses;

        public bool EveryoneIsReady => _loadStatuses.Values.All(x => x is true);

        public LoadObserver Instantiate(NetworkManager networkManager)
        {
            gameObject.SetActive(false);
            LoadObserver result = Instantiate(this);
            gameObject.SetActive(true);

            result._networkManager = networkManager;
            result.gameObject.SetActive(true);

            return result;
        }

        public bool GetClientStatus(ulong clientID)
        {
            if (_loadStatuses.TryGetValue(clientID, out bool result))
            {
                return result;
            }

            _loadStatuses.Add(clientID, true);

            return true;
        }

        private void Awake()
        {
            _networkManager.OnClientStarted += OnClientStart;
            _networkManager.OnClientStopped += OnClientStop;
        }

        private void OnDestroy()
        {
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
            if (_loadStatuses.ContainsKey(clientId) == false)
            {
                _loadStatuses.Add(clientId, true);
            }

            bool oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = true;
            StatusChanged?.Invoke(clientId, oldStatus, _loadStatuses[clientId]);
        }

        private void OnLoadStart(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            if (_loadStatuses.ContainsKey(clientId) == false)
            {
                _loadStatuses.Add(clientId, false);
            }

            bool oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = false;
            StatusChanged?.Invoke(clientId, oldStatus, _loadStatuses[clientId]);
        }
    }
}