using Core.Players;
using Core.PlayerTablets;
using UI.CommonScripts;
using UnityEngine;
using Zenject;

namespace UI.PlayerTablets
{
    public class Tabs : MonoBehaviour
    {
        [SerializeField] private Tab _emptyTab;
        
        [SerializeField] private Tab _bookedTab;

        [SerializeField] private PlayerTabletContainer _playerTabletContainer;

        public PlayerTablet PlayerTablet => _playerTabletContainer.PlayerTablet;

        private void OnEnable()
        {
            UpdateTabsActive();
            PlayerTablet.PlayerReference.ReferenceChanged += OnReferenceChange;
        }

        private void OnDisable()
        {
            PlayerTablet.PlayerReference.ReferenceChanged -= OnReferenceChange;
        }

        private void OnReferenceChange(Player oldValue, Player newValue)
        {
            UpdateTabsActive();
        }

        private void UpdateTabsActive()
        {
            if (PlayerTablet.PlayerReference.Reference == null)
            {
                _emptyTab.Enable();
                return;
            }

            _bookedTab.Enable();
        }
    }
}
