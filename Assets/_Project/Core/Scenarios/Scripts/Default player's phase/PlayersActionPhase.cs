using System.Linq;
using Core.PlayerTablets;
using Unity.Netcode;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersActionPhase : IChapter
    {
        private PlayerTabletList _playerTabletList;
        private ActionPointsGiver _actionPointsGiver;

        public event IChapter.EndedListener Ended;

        public PlayersActionPhase(PlayerTabletList playerTabletsList, ActionPointsGiver actionPointsGiver)
        {
            _playerTabletList = playerTabletsList;
            _actionPointsGiver = actionPointsGiver;
        }

        public void Begin()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            foreach (PlayerTablet playerTablet in _playerTabletList)
            {
                playerTablet.IsPassed.Value = false;
                playerTablet.IsPassed.Changed += OnIsPassedChange;
            }

            _actionPointsGiver.Give();
        }

        private void OnIsPassedChange(bool previousValue, bool newValue)
        {
            if (_playerTabletList.All(x => x.IsPassed.Value))
            {
                Ended?.Invoke(this);
            }
        }
    }
}
