using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace SelectionStarted
{
    [RequireComponent(typeof(PointerEvents))]
    public class CardSelectionToggle : MonoBehaviour
    {
        [SerializeField] private SelectedCardView _selectedCardView;
        
        [Inject] private CardsSelectionView _cardsSelection;

        private PointerEvents _pointerEvents;
        
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
            if (_cardsSelection.Contains(_selectedCardView))
            {
                _cardsSelection.Remove(_selectedCardView);
            }
            else
            {
                _cardsSelection.Add(_selectedCardView);
            }
        }
    }
}
