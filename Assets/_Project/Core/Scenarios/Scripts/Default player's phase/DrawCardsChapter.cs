using System.Threading.Tasks;
using Core.PlayerTablets;
using Unity.Netcode;

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
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            _ = DrawCards();
        }

        private async Task DrawCards()
        {
            foreach (PlayerTablet playerTablet in _playerTablets)
            {
                await playerTablet.ActionCardsDeck.DrawCards();
            }
            Ended?.Invoke(this);
        }
    }
}
