using System.Linq;
using Core.ActionsCards;
using Core.Selection.Cards;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace SelectionStarted
{
    [RequireComponent(typeof(PointerEvents))]
    public class CardSelectionToggle : MonoBehaviour
    {
        [SerializeField] private ActionCardContainer _actionCardContainer;
        
        [Inject] private CardsSelection _cardsSelection;

        private PointerEvents _pointerEvents;

        public ActionCard ActionCard => _actionCardContainer.ActionCard;

        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnPointerClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnPointerClick;
        }

        private void OnPointerClick(PointerEvents sender, PointerEventData eventData)
        {
            if (_cardsSelection.Contains(ActionCard))
            {
                _cardsSelection.Remove(ActionCard);
            }
            else
            {
                _cardsSelection.Add(ActionCard);
            }
        }
    }
}
