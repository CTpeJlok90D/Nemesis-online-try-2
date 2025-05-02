using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UI.Common;
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
        private bool _handIsSyncing;
        public PlayerTablet ActiveTablet { get; private set; }
        public ActionCardsDeck ActionsCardsDeck => ActiveTablet.ActionCardsDeck;
        private readonly Dictionary<ActionCard, List<ActionCardContainer>> _displayedActionCards = new();
        
        private void Awake()
        {
            ActiveTablet = _playerTabletList.Local;
        }

        private void Update()
        {
            if (_handIsSyncing == false && ActionsCardsDeck != null)
            {
                _ = SyncHand();
            }
        }

        private async UniTask SyncHand()
        {
            _handIsSyncing = true;
            _handSyncIndex++;
            int syncIndex = _handSyncIndex;
            
            IReadOnlyCollection<ActionCard> newHand = await ActionsCardsDeck.GetHand();

            if (syncIndex != _handSyncIndex)
            {
                _handIsSyncing = false;
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
            
            _handIsSyncing = false;
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
