using System;
using System.Collections.Generic;
using Core.Lobbies;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace UI.PlayerTablets
{
    public class PlayetTabletsElementsSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerTabletContainer _card_PREFAB;

        [SerializeField] private Transform _cardsParent;

        [Inject] private Lobby _lobby;

        private Dictionary<PlayerTablet, PlayerTabletContainer> _playerTabletInstances = new();

        private void OnEnable()
        {
            _lobby.PlayerCountChanged += OnPlayerAmountChange;
        }

        private void Start()
        {
            SpawnLobbyCards();
        }

        private void OnDisable()
        {
            _lobby.PlayerCountChanged -= OnPlayerAmountChange;
        }

        private void SpawnLobbyCards()
        {
            foreach (PlayerTablet tablet in _lobby.ActiveTablets)
            {
                _card_PREFAB.Instantiate(tablet, _cardsParent);
            }
        }

        private void OnPlayerAmountChange(Lobby.ChangedData changedData, int oldCount, int newCount)
        {
            foreach (PlayerTablet removed in changedData.RemovedTablets)
            {
                PlayerTabletContainer containerInstance = _playerTabletInstances[removed];
                Destroy(containerInstance.gameObject);
                _playerTabletInstances.Remove(removed);
            }

            foreach (PlayerTablet added in changedData.AddedTablets)
            {
                _playerTabletInstances.Add(added, _card_PREFAB.Instantiate(added, _cardsParent));
            }
        }
    }
}
