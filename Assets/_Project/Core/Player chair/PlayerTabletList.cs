using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.PlayerTablets
{
    public class PlayerTabletList : NetworkBehaviour, IEnumerable<PlayerTablet>
    {
        [SerializeField] private PlayerTablet _playerTablet_PREFAB;

        private NetworkList<NetworkObjectReference> _activeTablets;

        private int _oldCount;

        public PlayerTablet[] ActiveTablets => _activeTablets.ToEnumerable<PlayerTablet>().ToArray();

        private event NetworkList<NetworkObjectReference>.OnListChangedDelegate _activeTabletsSynced;

        public event NetworkList<NetworkObjectReference>.OnListChangedDelegate ActiveTabletsChanged
        {
            add 
            {
                _activeTabletsSynced += value;
                 _activeTablets.OnListChanged += value;
            }
            remove 
            {
                _activeTabletsSynced -= value;
                 _activeTablets.OnListChanged -= value;
            }
        }

        private void Awake()
        {
            _activeTablets = new();
            _oldCount = _activeTablets.Count;
        }

        private void Start()
        {
            SyncTablets();
        }

        private async void SyncTablets()
        {
            try
            {
                while (_activeTablets.Count == _oldCount)
                {
                    await Awaitable.NextFrameAsync();
                }

                NetworkListEvent<NetworkObjectReference> args = new();
                _activeTabletsSynced?.Invoke(args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public PlayerTablet AddTablet()
        {
            PlayerTablet prefabInstance = Instantiate(_playerTablet_PREFAB);
            DontDestroyOnLoad(prefabInstance);
            prefabInstance.NetworkObject.Spawn();
            _activeTablets.Add(prefabInstance.NetworkObject);

            return prefabInstance;
        }

        public PlayerTablet RemoveTablet()
        {
            PlayerTablet playerTablet = _activeTablets.ToEnumerable<PlayerTablet>().First();

            _activeTablets.Remove(playerTablet.NetworkObject);
            playerTablet.NetworkObject.Despawn(true);

            return playerTablet;
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
