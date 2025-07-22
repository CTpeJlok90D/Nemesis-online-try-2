using System.Collections.Generic;
using System.Linq;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.PlayerTablets
{
    public class PlayerTabletsElementsSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerTabletContainer _card_PREFAB;

        [SerializeField] private Transform _cardsParent;
        
        private NetworkManager NetworkManager => NetworkManager.Singleton;
        
        [Inject] private DiContainer _diContainer;

        private Dictionary<PlayerTablet, PlayerTabletContainer> _playerTabletInstances = new();

        private void OnEnable()
        {
            PlayerTablet.Spawned += OnActiveTabletsChange;
            PlayerTablet.Despawned += OnActiveTabletsChange;
            NetworkManager.OnClientStarted += OnClientStart;
            if (didStart)
            {
                UpdateCards();
            }
        }

        private void OnDisable()
        {
            PlayerTablet.Spawned -= OnActiveTabletsChange;
            PlayerTablet.Despawned -= OnActiveTabletsChange;
            if (NetworkManager != null)
            {
                NetworkManager.OnClientStarted -= OnClientStart;
            }
        }

        private void Start()
        {
            UpdateCards();
        }

        private void OnClientStart() => UpdateCards();
        private void OnActiveTabletsChange(PlayerTablet spawned) => UpdateCards();
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
            foreach (PlayerTablet tablet in PlayerTablet.Instances.OrderBy(x => x.OrderNumber.Value))
            {
                PlayerTabletContainer instance = _card_PREFAB.Instantiate(tablet, _diContainer, _cardsParent);
                _playerTabletInstances.Add(tablet, instance);
            }
        }
    }
}
