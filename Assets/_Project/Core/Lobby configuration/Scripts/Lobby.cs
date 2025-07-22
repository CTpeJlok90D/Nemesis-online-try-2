using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Core.Entities;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Lobbies
{
    public class Lobby : NetEntity<Lobby>
    {
        public delegate void PlayerCountChangedListener(ChangedData changedData, int oldCount, int newCount);

        [SerializeField] private LobbyConfiguration _defaultLobbyConfiguration;
        [SerializeField] private PlayerTablet _playerTablet_PREFAB;

        private NetVariable<LobbyConfiguration> _configuration;

        public event PlayerCountChangedListener PlayerCountChanged;

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
                _ = UpdateTablets();
            }
        }

        public void Awake()
        {
            _configuration = new(_defaultLobbyConfiguration);
        }

        public override void OnNetworkSpawn()
        {
            _configuration.Changed += OnConfigurationChange;
            _ = UpdateTablets();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _configuration.Changed -= OnConfigurationChange;
        }

        private void OnConfigurationChange(LobbyConfiguration previousValue, LobbyConfiguration newValue)
        {
            _ = UpdateTablets();
        }

        private async UniTask UpdateTablets()
        {
            if (IsServer == false)
            {
                return;
            }

            int oldPlayersCount = PlayerTablet.Instances.Count;
            List<PlayerTablet> addedTablets = new();
            List<PlayerTablet> removedTablets = new();

            while (PlayerTablet.Instances.Count > _configuration.Value.PlayersCount)
            {
                PlayerTablet tabletToRemove = PlayerTablet.Instances.First();
                removedTablets.Add(tabletToRemove);
                tabletToRemove.NetworkObject.Despawn();
            }

            while (PlayerTablet.Instances.Count < _configuration.Value.PlayersCount)
            {
                PlayerTablet playerTablet = Instantiate(_playerTablet_PREFAB);
                playerTablet.NetworkObject.Spawn();
                addedTablets.Add(playerTablet);
            }

            ChangedData data = new()
            {
                AddedTablets = addedTablets,
                RemovedTablets = removedTablets
            };

            PlayerCountChanged?.Invoke(data, oldPlayersCount, PlayerTablet.Instances.Count);
        }

        public record ChangedData
        {
            public IReadOnlyCollection<PlayerTablet> AddedTablets;
            public IReadOnlyCollection<PlayerTablet> RemovedTablets;
        }
    }
}
