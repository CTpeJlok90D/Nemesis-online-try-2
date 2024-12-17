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
        public delegate void StatusChangedListener(ulong clientId, Status oldStatus, Status newStatus);

        private SerializedDictionary<ulong, Status> _loadStatuses = new();

        private NetworkManager _networkManager;

        public event StatusChangedListener StatusChanged;

        public IReadOnlyDictionary<ulong, Status> LoadStatuses => _loadStatuses;

        public bool EveryoneIsReady => _loadStatuses.Values.All(x => x is Status.Ready);

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
            if (_loadStatuses.TryGetValue(clientID, out Status result))
            {
                return result;
            }

            _loadStatuses.Add(clientID, Status.Ready);

            return Status.Ready;
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
                _loadStatuses.Add(clientId, Status.Ready);
            }

            Status oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = Status.Ready;
            StatusChanged?.Invoke(clientId, oldStatus, _loadStatuses[clientId]);
        }

        private void OnLoadStart(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            if (_loadStatuses.ContainsKey(clientId) == false)
            {
                _loadStatuses.Add(clientId, Status.NotReady);
            }

            Status oldStatus = _loadStatuses[clientId];

            _loadStatuses[clientId] = Status.NotReady;
            StatusChanged?.Invoke(clientId, oldStatus, _loadStatuses[clientId]);
        }

        public enum Status
        {
            Ready,
            NotReady
        }
    }
}