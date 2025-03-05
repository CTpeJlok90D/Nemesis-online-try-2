using System;
using System.Linq;
using Core.ActionsCards;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace Core.Selection.Cards
{
    public class CardsSelection : Selection<ActionCard>
    {
        
    }

    public class SelectionCardView : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private ActionCardContainer _actionCardContainer;

        [Inject] private CardsSelection _cardsSelection;

        private void OnEnable()
        {
            UpdateCardSelectionView();
            _cardsSelection.SelectionChanged += UpdateCardSelectionView;
        }

        private void OnDisable()
        {
            _cardsSelection.SelectionChanged -= UpdateCardSelectionView;
        }

        private void UpdateCardSelectionView(ISelection sender) => UpdateCardSelectionView();
        private void UpdateCardSelectionView()
        {
            _target.SetActive(_cardsSelection.Contains(_actionCardContainer.ActionCard));
        }
    }
}
