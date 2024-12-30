using Core.PlayerTablets;

namespace Core.Scenarios.PlayersPhase
{
    public class DrawCardsChapter : IChapter
    {
        private PlayerTabletList _playerTablets;

        public event IChapter.EndedListener Ended;

        public DrawCardsChapter(PlayerTabletList playerTabletsList)
        {
            _playerTablets = playerTabletsList;
        }

        public void Begin()
        {
            foreach (PlayerTablet playerTablet in _playerTablets)
            {
                playerTablet.ActionCardsDeck.DrawCards();
            }
            Ended?.Invoke(this);
        }
    }
}
