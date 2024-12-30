using Core.LoadObservers;
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

        private bool _lastValue;

        private void OnEnable()
        {
            _loadObserver.StatusChanged += OnStatusChange;
            _animator.SetTrigger(_showTrigger);
        }

        private void OnDisable()
        {
            _loadObserver.StatusChanged -= OnStatusChange;
        }

        private void OnStatusChange(ulong clientId, LoadObserver.Status oldStatus, LoadObserver.Status newStatus)
        {
            UpdateScreen();   
        }

        private void UpdateScreen()
        {
            _lastValue = _loadObserver.EveryoneIsReady;

            if (_lastValue == false)
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
