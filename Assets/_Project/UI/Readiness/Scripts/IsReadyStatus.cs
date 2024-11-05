using Core.Players;
using Core.Readiness;
using UI.CommonScripts;
using UnityEngine;

namespace UI.Readiness
{
    public class IsReadyStatus : PlayerContainerListener
    {
        [SerializeField] private Tab _readyTab;
        [SerializeField] private Tab _notReadyTab;
        private Preparation _playerPrepearing;

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_playerPrepearing != null)
            {
                _playerPrepearing.IsReady.Changed -= OnReadyChange;
            }
        }

        protected override void OnPlayerChange(Player player)
        {
            if (_playerPrepearing != null)
            {
                _playerPrepearing.IsReady.Changed -= OnReadyChange;
            }

            _playerPrepearing = player.GetComponent<Preparation>();

            if (_playerPrepearing != null)
            {
                _playerPrepearing.IsReady.Changed += OnReadyChange;

                ValidateStatus();
            }
        }

        private void OnReadyChange(bool previousValue, bool newValue) => ValidateStatus();
        private void ValidateStatus()
        {
            if (_playerPrepearing.IsReady.Value)
            {
                _readyTab.Enable();
            }
            else
            {
                _notReadyTab.Enable();
            }
        }
    }
}