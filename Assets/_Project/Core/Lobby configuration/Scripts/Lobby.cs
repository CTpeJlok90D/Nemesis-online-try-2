using System;
using System.Collections.Generic;
using System.Linq;
using Core.PlayerTablets;
using Unity.Netcode;
using Unity.Netcode.Custom;
using Unity.Profiling;
using UnityEngine;

namespace Core.Lobbies
{
    public class Lobby : NetworkBehaviour
    {
        public delegate void PlayerCountChangedListener(ChangedData changedData, int oldCount, int newCount);

        [SerializeField] private PlayerTablet _playerTablet_PREFAB;
        [SerializeField] private LobbyConfiguration _defaultLobbyConfiguration;

        private NetVariable<LobbyConfiguration> _configuration;

        private NetObjectList<PlayerTablet> _activeTablets;

        public event PlayerCountChangedListener PlayerCountChanged;

        public PlayerTablet[] ActiveTablets => _activeTablets.ToArray();

        public LobbyConfiguration Configuration 
        {
            get 
            {
                return _configuration.Value;
            }
            set
            {
                if (NetworkManager.IsServer == false)
                {
                    throw new NotServerException("Only server can change lobby configuration");
                }

                if (value.PlayersCount <= 0)
                {
                    throw new ArgumentException("Player count can't be zero or lesser");
                }

                _configuration.Value = value;
                UpdateTablets();
            }
        }

        public void Awake()
        {
            _configuration = new(_defaultLobbyConfiguration);
            _activeTablets = new();
        }

        public void Start()
        {
            UpdateTablets();
            NetworkManager.OnServerStarted += OnServerStart;
        }

        private void OnEnable()
        {
            _configuration.Changed += OnConfigurationChange;
            if (didStart)
            {
                NetworkManager.OnServerStarted += OnServerStart;
            }
        }

        private void OnDisable()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnServerStart;                
            }
            _configuration.Changed -= OnConfigurationChange;
        }

        private void OnServerStart()
        {
            UpdateTablets();
        }

        private void OnConfigurationChange(LobbyConfiguration previousValue, LobbyConfiguration newValue)
        {
            UpdateTablets();
        }

        private void UpdateTablets()
        {
            if (IsServer == false)
            {
                return;
            }

            int oldPlayersCount = _activeTablets.Count;
            List<PlayerTablet> addedTablets = new();
            List<PlayerTablet> removedTablets = new();

            while (_activeTablets.Count > _configuration.Value.PlayersCount)
            {
                PlayerTablet playerTablet = _activeTablets.First();

                removedTablets.Add(playerTablet);

                _activeTablets.Remove(playerTablet);
                playerTablet.NetworkObject.Despawn(true);
            }

            while (_activeTablets.Count < _configuration.Value.PlayersCount)
            {
                PlayerTablet prefabInstance = Instantiate(_playerTablet_PREFAB);
                DontDestroyOnLoad(prefabInstance);
                prefabInstance.NetworkObject.Spawn();

                _activeTablets.Add(prefabInstance);
                addedTablets.Add(prefabInstance);
            }

            ChangedData data = new()
            {
                AddedTablets = addedTablets,
                RemovedTablets = removedTablets
            };

            PlayerCountChanged?.Invoke(data, oldPlayersCount, _activeTablets.Count);
        }

        public record ChangedData
        {
            public IReadOnlyCollection<PlayerTablet> AddedTablets;
            public IReadOnlyCollection<PlayerTablet> RemovedTablets;
        }
    }
}
