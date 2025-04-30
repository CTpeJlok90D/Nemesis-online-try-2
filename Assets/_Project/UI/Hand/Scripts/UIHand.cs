using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;
using Core.PlayerTablets;
using UI.Common;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace UI.Hands
{
    [DefaultExecutionOrder(5)]
    public class UIHand : MonoBehaviour
    {
        [field: SerializeField] private ActionCardContainer _uiActionCard_PREFAB;

        [field: SerializeField] private Transform _parent;

        [Inject] private PlayerTabletList _playerTabletList;

        [Inject] private DiContainer _diContainer;

        private int _handSyncIndex;

        private ActionCardsDeck _oldActionCards;

        public PlayerTablet ActiveTablet { get; private set; }

        public ActionCardsDeck ActionsCardsDeck => ActiveTablet.ActionCardsDeck;

        private Dictionary<ActionCard, List<ActionCardContainer>> _displayedActionCards = new();
        
        private void Awake()
        {
            ActiveTablet = _playerTabletList.Local;
        }

        private void OnEnable()
        {
            ActiveTablet.PawnLinked += OnPawnLink;

            if (ActionsCardsDeck != null)
            {
                ActionsCardsDeck.HandChanged += OnHandChange;
                _ = SyncHand();
            }
        }

        private void OnPawnLink(PlayerTablet sender)
        {
            if (_oldActionCards != null)
            {
                _oldActionCards.HandChanged -= OnHandChange;
            }
            
            ActionsCardsDeck.HandChanged += OnHandChange;
            _oldActionCards = ActionsCardsDeck;
            _ = SyncHand();
        }

        private void OnDisable()
        {
            ActiveTablet.PawnLinked -= OnPawnLink;
            if (ActionsCardsDeck != null)
            {
                ActionsCardsDeck.HandChanged -= OnHandChange;
            }
        }

        private void OnHandChange(NetScriptableObjectList4096<ActionCard> sender)
        {
            _ = SyncHand();
        }

        private async Task SyncHand()
        {
            try
            {
                _handSyncIndex++;
                int syncIndex = _handSyncIndex;
                IReadOnlyCollection<ActionCard> newHand = await ActionsCardsDeck.GetHand();

                if (syncIndex != _handSyncIndex)
                {
                    return;
                }

                foreach ((ActionCard actionCard, List<ActionCardContainer> uIActionCard) in _displayedActionCards)
                {
                    if (_displayedActionCards.ContainsKey(actionCard) == false)
                    {
                        _displayedActionCards.Add(actionCard, new());
                    }

                    while (newHand.Count(x => x == actionCard) < _displayedActionCards[actionCard].Count)
                    {
                        RemoveCardFromDisplay(actionCard);
                    }
                }

                foreach (ActionCard actionCard in newHand)
                {
                    if (_displayedActionCards.ContainsKey(actionCard) == false)
                    {
                        _displayedActionCards.Add(actionCard, new());
                    }

                    while (newHand.Count(x => x == actionCard) > _displayedActionCards[actionCard].Count)
                    {
                        AddCardToDisplay(actionCard);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void RemoveCardFromDisplay(ActionCard actionCard)
        {
            ActionCardContainer uIActionCard = _displayedActionCards[actionCard].First();
            _displayedActionCards[actionCard].Remove(uIActionCard);
            
            if (uIActionCard.TryGetComponent(out IDestroyable component))
            {
                component.Destroy();
            }
            else
            {
                Destroy(uIActionCard.gameObject);
            }
        }

        private void AddCardToDisplay(ActionCard actionCard)
        {
            ActionCardContainer actionCardInstance = _uiActionCard_PREFAB.Instantiate(actionCard, _diContainer, _parent);
            _displayedActionCards[actionCard].Add(actionCardInstance);
        }
    }
}
