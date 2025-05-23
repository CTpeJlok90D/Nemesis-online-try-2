using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.PlayerActions;
using Core.PlayerTablets;
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

        public PlayerTablet ActiveTablet => _playerTabletsList.ElementAt(_activePlayerIndex.Value);

        private void Awake()
        {
            _firstPlayerIndex = new();
            _activePlayerIndex = new();
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
                    index++;
                    if (index >= _playerTabletsList.Count())
                    {
                        index = 0;
                    }
                    
                    tablet = _playerTabletsList.ElementAt(index);
                } while (tablet.IsPassed.Value);

                _activePlayerIndex.Value = index;

                PlayerActionExecutor.Instance.Executor = tablet;

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
            if (_playerTabletsList.Any() == false)
            {
                return;
            }
            
            ActiveTablet.ActionCount.Changed -= OnActionPointsCountChange;
            ActiveTablet.IsPassed.Changed -= OnActivePlayerPass;
            
            Give();
        }

        private async void OnActionPointsCountChange(int previousValue, int newValue)
        {
            try
            {
                if (newValue == 0)
                {
                    ActiveTablet.ActionCount.Changed -= OnActionPointsCountChange;
                    ActiveTablet.IsPassed.Changed -= OnActivePlayerPass;

                    IReadOnlyCollection<ActionCard> hand = await ActiveTablet.ActionCardsDeck.GetHand();

                    if (hand.Any() == false)
                    {
                        ActiveTablet.IsPassed.Value = true;
                    }
                    
                    Give();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

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
