using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.PlayerTablets;
using ModestTree;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.PlayersPhase
{
    public class ActionPointsGiver : NetworkBehaviour
    {
        [Inject] private PlayerTabletList _playerTabletsList;

        private NetVariable<int> _firstPlayerIndex;

        private NetVariable<int> _activePlayerIndex;

        private int _actionPointsToGive = 2;

        private bool _isFistMove;

        private void Awake()
        {
            _firstPlayerIndex = new();
        }

        public void Give()
        {
            try
            {
                if (_playerTabletsList.All(x => x.IsPassed.Value))
                {
                    return;
                }

                PlayerTablet tablet = null;
                int index = _activePlayerIndex.Value;

                do 
                {
                    tablet = _playerTabletsList.ElementAt(index);
                    index++;
                } while (tablet.IsPassed.Value);

                _activePlayerIndex.Value = index;

                tablet.ActionCount.Value = _actionPointsToGive;
                tablet.ActionCount.Changed += OnActionPointsCountChange;
                tablet.IsPassed.Changed += OnActivePlayerPass;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnActivePlayerPass(bool previousValue, bool newValue)
        {
            PlayerTablet activeTablet = _playerTabletsList.ElementAt(_activePlayerIndex.Value);

            activeTablet.ActionCount.Changed -= OnActionPointsCountChange;
            activeTablet.IsPassed.Changed -= OnActivePlayerPass;

            _activePlayerIndex.Value++;
            Give();
        }

        private async void OnActionPointsCountChange(int previousValue, int newValue)
        {
            if (newValue == 0)
            {
                PlayerTablet activeTablet = _playerTabletsList.ElementAt(_activePlayerIndex.Value);

                activeTablet.ActionCount.Changed -= OnActionPointsCountChange;
                activeTablet.IsPassed.Changed -= OnActivePlayerPass;

                IReadOnlyCollection<ActionCard> hand = await activeTablet.ActionCardsDeck.GetHand();

                if (hand.IsEmpty())
                {
                    activeTablet.IsPassed.Value = true;
                }

                _activePlayerIndex.Value++;
            }

            Give();
        }

        public void MoveFirstPlayer()
        {
            if (_isFistMove)
            {
                _isFistMove = false;
            }

            if (_firstPlayerIndex.Value + 1 >= _playerTabletsList.Count())
            {
                _firstPlayerIndex.Value = 0;
            }
            else
            {
                _firstPlayerIndex.Value++;
            }

            _activePlayerIndex.Value = _firstPlayerIndex.Value;
        }
    }
}
