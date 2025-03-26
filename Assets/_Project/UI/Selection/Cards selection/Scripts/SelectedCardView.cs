using System.Linq;
using Core.ActionsCards;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace SelectionStarted
{
    [DefaultExecutionOrder(1)]
    public class SelectedCardView : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private ActionCardContainer _actionCardContainer;
        
        [Inject] private CardsSelectionView _cardsSelectionView;

        public ActionCard ActionCard => _actionCardContainer.ActionCard; 
        
        public string ActionCardID => ActionCard.ID;

        private void Awake()
        {
            UpdateIsSelected();
        }

        private void OnEnable()
        {
            _cardsSelectionView.SelectionChanged += OnSelectionChange;
        }

        private void OnDisable()
        {
            _cardsSelectionView.SelectionChanged -= OnSelectionChange;
        }

        private void OnSelectionChange(ISelection sender)
        {
            UpdateIsSelected();
        }

        private void UpdateIsSelected()
        {
            _target.SetActive(_cardsSelectionView.Contains(this));
        }
    }
}