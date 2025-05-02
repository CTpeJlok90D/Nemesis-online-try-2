using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ActionsCards;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

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

        private async UniTask DrawCards()
        {
            foreach (PlayerTablet playerTablet in _playerTablets)
            {
                IReadOnlyCollection<ActionCard> takenCards = await playerTablet.ActionCardsDeck.DrawCards();
                Debug.Log($"{playerTablet} took cards: {string.Join(", ",takenCards)}");
            }
            Ended?.Invoke(this);
        }
    }
}
