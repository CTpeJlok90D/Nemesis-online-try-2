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
            _loadObserver.StatusChanged += OnLoadStatusChange;
        }

        private void OnLoadStatusChange(ulong clientId, bool oldStatus, bool newStatus)
        {
            if (_loadObserver.EveryoneIsReady)
            {
                _loadObserver.StatusChanged -= OnLoadStatusChange;
                Ended?.Invoke(this);
            }
        }
    }
}
