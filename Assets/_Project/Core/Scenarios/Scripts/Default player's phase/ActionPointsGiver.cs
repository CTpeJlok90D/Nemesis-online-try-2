using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Maps.CharacterPawns;
using Core.OrderNumberDestributors;
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
        private NetVariable<int> _firstPlayerIndex;

        private NetVariable<int> _activePlayerIndex;

        private int _actionPointsToGive = 2;

        private bool _isFistMove;

        private PlayerTablet[] OrderedPlayerTablets =>
            PlayerTablet.ActiveTablets.OrderBy(x => x.OrderNumber.Value).ToArray();

        public PlayerTablet ActiveTablet => OrderedPlayerTablets[_activePlayerIndex.Value];

        public event IReadOnlyReactiveField<int>.ChangedListener ActiveTabletIndexChanged
        {
            add => _activePlayerIndex.Changed += value;
            remove => _activePlayerIndex.Changed -= value;
        }

        private void Awake()
        {
            _firstPlayerIndex = new();
            _activePlayerIndex = new();
        }

        private void OnEnable()
        {
            CharacterPawn.Despawned += OnCharacterDespawn;
        }

        private void OnDisable()
        {
            CharacterPawn.Despawned -= OnCharacterDespawn;
        }

        private void OnCharacterDespawn(CharacterPawn despawned)
        {
            PlayerTablet playerTablet = OrderedPlayerTablets.FirstOrDefault(x => x.CharacterPawn == despawned);
            if (playerTablet == null)
            {
                return;
            }
            playerTablet.Pass();
        }

        public void Give()
        {
            try
            {
                if (PlayerTablet.ActiveTablets.All(x => x.IsPassed.Value))
                {
                    return;
                }

                PlayerTablet tablet = null;
                int index = _activePlayerIndex.Value;
                
                do 
                {
                    index++;
                    if (index >= OrderedPlayerTablets.Length)
                    {
                        index = 0;
                    }
                    
                    tablet = OrderedPlayerTablets[index];
                } while (tablet.IsPassed.Value);

                _activePlayerIndex.Value = index;

                PlayerActionExecutor.Singleton.Executor = tablet;

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
            if (PlayerTablet.ActiveTablets.Any() == false)
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

            if (_firstPlayerIndex.Value + 1 >= OrderedPlayerTablets.Length)
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
