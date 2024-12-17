using System;
using AYellowpaper;
using Core.LoadObservers;
using Core.Players;
using UnityEngine;
using Zenject;

namespace UI.WaitOtherPlayers
{
    public class PlayerLoadingIndicator : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IContainsPlayer> _player;

        [SerializeField] private GameObject _indicator;

        [Inject] private LoadObserver _loadObserver;

        public Player Player => _player.Value.Player;

        private void OnEnable()
        {
            _loadObserver.StatusChanged += OnStatusChange;
            UpdateIndicator();
        }

        private void OnDisable()
        {
            _loadObserver.StatusChanged -= OnStatusChange;
        }

        private void OnStatusChange(ulong clientId, LoadObserver.Status oldStatus, LoadObserver.Status newStatus) => UpdateIndicator();
        private void UpdateIndicator()
        {
            _indicator.SetActive(Player != null && _loadObserver.GetClientStatus(Player.OwnerClientId) == LoadObserver.Status.NotReady);
        }
    }
}
