using Core.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI
{
    [DefaultExecutionOrder(-1)]
    public class PlayerList : MonoBehaviour
    {
        [SerializeField] private PlayerContainer _playerListElement_PREFAB;
        [SerializeField] private Transform _elementsParent;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        private Dictionary<Player, PlayerContainer> _listElementsInstances = new();

        private void Awake()
        {
            _playerListElement_PREFAB.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (didStart)
            {
                NetworkManager.OnClientStarted += OnClientStart;
                NetworkManager.OnClientStopped += OnClientStopped;
            }
            
            Player.Join += OnPlayerJoin;
            Player.Left += OnPlayerLeft;

            ClearList();
            InitializeList();
        }

        private void Start()
        {
            if (didStart)
            {
                NetworkManager.OnClientStarted += OnClientStart;
                NetworkManager.OnClientStopped += OnClientStopped;
            }
        }

        private void OnDisable()
        {
            Player.Join -= OnPlayerJoin;
            Player.Left -= OnPlayerLeft;

            if (NetworkManager != null)
            {
                NetworkManager.OnClientStarted -= OnClientStart;
                NetworkManager.OnClientStopped -= OnClientStopped;
            }
        }

        private void OnClientStopped(bool obj)
        {
            ClearList();
        }

        private void OnClientStart()
        {
            InitializeList();
        }

        private void OnPlayerLeft(Player player) => RemovePlayerFromList(player);
        private void RemovePlayerFromList(Player player)
        {
            try
            {
                PlayerContainer playerContainer = _listElementsInstances[player];
                _listElementsInstances.Remove(player);

                Destroy(playerContainer.gameObject);
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"Player with id {player.OwnerClientId} not found in list", this);
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }

        private void OnPlayerJoin(Player player) => AddPlayerToList(player);
        private void AddPlayerToList(Player player)
        {
            PlayerContainer instance = Instantiate(_playerListElement_PREFAB, _elementsParent).Init(player);
            instance.gameObject.SetActive(true);

            _listElementsInstances.Add(player, instance);
        }

        private void ClearList() 
        {
            foreach ((Player key, PlayerContainer value) in _listElementsInstances.ToArray()) 
            {
                _listElementsInstances.Remove(key);
                Destroy(value.gameObject);
            }
        }

        private void InitializeList()
        {
            foreach (Player player in Player.List)
            {
                AddPlayerToList(player);
            }
        }
    }
}
