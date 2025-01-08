using Core.LoadObservers;
using Core.Players;
using UnityEngine;
using Zenject;

namespace UI.WaitOtherPlayers
{
    public class WaitOtherPlayersScreen : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private string _hideTrigger = "Hide";

        [SerializeField] private string _showTrigger = "Show";

        [SerializeField] private string _loadingAnimationStateName = "Loading";

        [Inject] private LoadObserver _loadObserver;

        private bool _lastValue;

        private void Start()
        {
            _loadObserver.StatusChanged += OnStatusChange;
            if (Player.List.Count > 1)
            {
                _animator.Play(_loadingAnimationStateName);
            }
        }

        private void OnDestroy()
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
