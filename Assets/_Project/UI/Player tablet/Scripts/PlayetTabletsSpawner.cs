using System;
using System.Collections.Generic;
using System.Linq;
using Core.Lobbies;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.PlayerTablets
{
    public class PlayetTabletsElementsSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerTabletContainer _card_PREFAB;

        [SerializeField] private Transform _cardsParent;

        [Inject] private PlayerTabletList _playetTabletList;

        [Inject] private NetworkManager _networkManager;

        private Dictionary<PlayerTablet, PlayerTabletContainer> _playerTabletInstances = new();

        private void OnEnable()
        {
            _playetTabletList.ActiveTabletsChanged += OnActiveTabletsChange;
            _networkManager.OnClientStarted += OnClientStart;
            UpdateCards();
        }

        private void OnDisable()
        {
            _playetTabletList.ActiveTabletsChanged -= OnActiveTabletsChange;
            _networkManager.OnClientStarted -= OnClientStart;
        }

        private void OnClientStart() => UpdateCards();

        private void OnActiveTabletsChange(NetworkListEvent<NetworkObjectReference> changeEvent) => UpdateCards();

        private void UpdateCards()
        {
            DestroyCards();
            SpawnLobbyCards();
        }

        private void DestroyCards()
        {
            foreach ((PlayerTablet tablet, PlayerTabletContainer container) in _playerTabletInstances)
            {
                Destroy(container.gameObject);
            }

            _playerTabletInstances.Clear();
        }

        private void SpawnLobbyCards()
        {
            foreach (PlayerTablet tablet in _playetTabletList.ActiveTablets.OrderBy(x => x.OrderNumber.Value))
            {
                PlayerTabletContainer instance = _card_PREFAB.Instantiate(tablet, _cardsParent);
                _playerTabletInstances.Add(tablet, instance);
            }
        }
    }
}
