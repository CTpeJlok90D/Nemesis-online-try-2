using System;
using System.Collections.Generic;
using System.Linq;
using Core.PlayerTablets;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class Lobby : NetworkBehaviour
    {
        public delegate void PlayerCountChangedListener(ChangedData changedData, int oldCount, int newCount);

        [SerializeField] private LobbyConfiguration _defaultLobbyConfiguration;

        private NetVariable<LobbyConfiguration> _configuration;

        public event PlayerCountChangedListener PlayerCountChanged;

        private PlayerTabletList _playerTabletList;

        [Inject] private NetworkManager _networkManager;

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

        public Lobby Instantiate(PlayerTabletList playerTabletList, NetworkManager networkManager)
        {
            gameObject.SetActive(false);
            Lobby result = Instantiate(this);
            gameObject.SetActive(true);

            result._playerTabletList = playerTabletList;
            result._networkManager = networkManager;
            result.gameObject.SetActive(true);
            return result;
        }

        public void Awake()
        {
            _configuration = new(_defaultLobbyConfiguration);
        }

        public override void OnNetworkSpawn()
        {
            _configuration.Changed += OnConfigurationChange;
            UpdateTablets();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _configuration.Changed -= OnConfigurationChange;
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

            if (_playerTabletList.IsSpawned == false)
            {
                _playerTabletList.NetworkObject.Spawn();
            }
            
            _playerTabletList.Clear();

            int oldPlayersCount = _playerTabletList.ActiveTablets.Length;
            List<PlayerTablet> addedTablets = new();
            List<PlayerTablet> removedTablets = new();

            while (_playerTabletList.ActiveTablets.Length > _configuration.Value.PlayersCount)
            {
                PlayerTablet removedTablet = _playerTabletList.Remove();
                removedTablets.Add(removedTablet);
            }

            while (_playerTabletList.ActiveTablets.Length < _configuration.Value.PlayersCount)
            {
                PlayerTablet playerTablet = _playerTabletList.Add();
                addedTablets.Add(playerTablet);
            }

            ChangedData data = new()
            {
                AddedTablets = addedTablets,
                RemovedTablets = removedTablets
            };

            PlayerCountChanged?.Invoke(data, oldPlayersCount, _playerTabletList.ActiveTablets.Length);
        }

        public record ChangedData
        {
            public IReadOnlyCollection<PlayerTablet> AddedTablets;
            public IReadOnlyCollection<PlayerTablet> RemovedTablets;
        }
    }
}
