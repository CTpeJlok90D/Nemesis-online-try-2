using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.PlayerTablets
{
    public class PlayerTabletList : NetworkBehaviour, IEnumerable<PlayerTablet>
    {
        [SerializeField] private PlayerTablet _playerTablet_PREFAB;
        private NetworkList<NetworkObjectReference> _activeTablets;
        private Starter.Activator _activator;
        private NetworkManager _networkManager;

        public PlayerTablet[] ActiveTablets => _activeTablets.ToEnumerable<PlayerTablet>().ToArray();
        public PlayerTablet Local => _activeTablets.ToEnumerable<PlayerTablet>().FirstOrDefault(x => x.Player.IsLocalPlayer);
        private event NetworkList<NetworkObjectReference>.OnListChangedDelegate ActiveTabletsSynced;

        public delegate void TabletRemovedHandler(PlayerTabletList sender, PlayerTablet tablet);
        public event TabletRemovedHandler TabletRemoved;

        public PlayerTablet FindTabletByPlayerNickname(string playerName)
        {
            PlayerTablet tablet = ActiveTablets.Where(x => x.Player != null).FirstOrDefault(x => x.NicknameContainer.Value == playerName);
            if (tablet == null)
            {
                throw new InvalidOperationException($"Player {playerName} not found");
            }
            
            return tablet;
        } 

        public event NetworkList<NetworkObjectReference>.OnListChangedDelegate ActiveTabletsChanged
        {
            add 
            {
                ActiveTabletsSynced += value;
                 _activeTablets.OnListChanged += value;
            }
            remove 
            {
                ActiveTabletsSynced -= value;
                 _activeTablets.OnListChanged -= value;
            }
        }

        public PlayerTabletList Init(Starter.Activator activator, NetworkManager networkManager)
        {
            _activator = activator;
            _networkManager = networkManager;

            return this;
        }

        private void Awake()
        {
            _activeTablets = new();

            // По какой то причине Unity автоматически не заполняет это поле
            Type type = typeof(NetworkVariableBase);
            FieldInfo fieldInfo = type.GetField("m_NetworkBehaviour", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(_activeTablets, this);
        }

        private void OnEnable()
        {
            _activator.GameActivated += OnGameActive;
        }

        private void OnDisable()
        {
            _activator.GameActivated -= OnGameActive;
        }

        public void Clear()
        {
            _activeTablets.Clear();
        }

        private void OnGameActive()
        {
            NetworkObject[] objectToRemove = ActiveTablets
                .Where(x => x.PlayerReference.Reference == null)
                .Select(x => x.NetworkObject)
                .ToArray();

            foreach (NetworkObject netObject in objectToRemove)
            {
                _activeTablets.Remove(netObject);
                netObject.Despawn(true);
            }
        }

        protected override void OnNetworkPostSpawn()
        {
            NetworkListEvent<NetworkObjectReference> args = new();
            ActiveTabletsSynced?.Invoke(args);   
        }

        public PlayerTablet Add()
        {
            PlayerTablet prefabInstance = Instantiate(_playerTablet_PREFAB);
            DontDestroyOnLoad(prefabInstance);
            prefabInstance.NetworkObject.Spawn();
            NetworkObjectReference networkObjectReference = new(prefabInstance.NetworkObject);
            _activeTablets.Add(networkObjectReference);

            return prefabInstance;
        }

        public PlayerTablet Remove()
        {
            PlayerTablet playerTablet = _activeTablets.ToEnumerable<PlayerTablet>().First();

            _activeTablets.Remove(playerTablet.NetworkObject);
            playerTablet.NetworkObject.Despawn();

            return playerTablet;
        }

        public bool Remove(PlayerTablet playerTablet)
        {
            bool result = _activeTablets.Remove(playerTablet.NetworkObject);
            if (result)
            {
                playerTablet.Pass();
                TabletRemoved?.Invoke(this, playerTablet);
                
                playerTablet.NetworkObject.Despawn();
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ActiveTablets.GetEnumerator();
        }

        IEnumerator<PlayerTablet> IEnumerable<PlayerTablet>.GetEnumerator()
        {
            foreach (PlayerTablet tablet in ActiveTablets)
            {
                yield return tablet;
            }
        }
    }
}
