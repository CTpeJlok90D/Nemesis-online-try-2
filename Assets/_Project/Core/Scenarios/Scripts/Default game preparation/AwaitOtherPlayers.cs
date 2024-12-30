using Core.LoadObservers;

namespace Core.Scenarios
{
    public class AwaitOtherPlayers : IChapter
    {
        private LoadObserver _loadObserver;

        public AwaitOtherPlayers(LoadObserver observer)
        {
            _loadObserver = observer;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            if (_loadObserver.EveryoneIsReady)
            {
                Ended?.Invoke(this);
                return;
            }
            _loadObserver.StatusChanged += OnLoadStatusChange;
        }

        private void OnLoadStatusChange(ulong clientId, LoadObserver.Status oldStatus, LoadObserver.Status newStatus)
        {
            if (_loadObserver.EveryoneIsReady)
            {
                _loadObserver.StatusChanged -= OnLoadStatusChange;
                Ended?.Invoke(this);
            }
        }
    }
}
