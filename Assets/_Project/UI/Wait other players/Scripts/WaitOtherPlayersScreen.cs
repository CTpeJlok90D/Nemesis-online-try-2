using System.Linq;
using Core.LoadObservers;
using UI.Loading;
using UnityEngine;
using Zenject;

namespace UI.WaitOtherPlayers
{
    public class WaitOtherPlayersScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _screen;

        [Inject] private LoadObserver _loadObserver;

        [Inject] private LoadScreen _loadScreen;

        private void OnEnable()
        {
            _loadObserver.StatusChanged += OnStatusChange;
        }

        private void OnDisable()
        {
            _loadObserver.StatusChanged -= OnStatusChange;
        }

        private void OnStatusChange(ulong clientId, bool oldStatus, bool newStatus)
        {
            bool result = _loadObserver.LoadStatuses.Values.All(x => x == false);
            _screen.SetActive(result);
            if (result)
            {
                _loadScreen.Show();
            }
            else
            {
                _loadScreen.Hide();
            }
        }
    }
}
