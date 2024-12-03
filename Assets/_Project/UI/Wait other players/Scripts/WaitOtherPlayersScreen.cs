using System.Linq;
using Core.LoadObservers;
using UI.Loading;
using UnityEngine;
using Zenject;

namespace UI.WaitOtherPlayers
{
    public class WaitOtherPlayersScreen : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private string _hideTrigger = "Hide";

        [SerializeField] private string _showTrigger = "Show";

        [Inject] private LoadObserver _loadObserver;

        private void OnEnable()
        {
            _loadObserver.StatusChanged += OnStatusChange;
            UpdateScreen();
        }

        private void OnDisable()
        {
            _loadObserver.StatusChanged -= OnStatusChange;
        }

        private void OnStatusChange(ulong clientId, bool oldStatus, bool newStatus)
        {
            UpdateScreen();   
        }

        private void UpdateScreen()
        {
            if (_loadObserver.EveryoneIsReady == false)
            {
                _animator.SetTrigger(_showTrigger);
            }
            else
            {
                _animator.SetTrigger(_hideTrigger);
            }
        }
    }
}
