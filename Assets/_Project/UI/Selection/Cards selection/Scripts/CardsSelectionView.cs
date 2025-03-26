using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Selection.Cards;
using Core.SelectionBase;
using UnityEngine;

namespace SelectionStarted
{
    public class CardsSelectionView : List<SelectedCardView>, ISelection
    {
        private CardsSelection _cardsSelection;
        
        public event ISelection.SelectionChangedHandler SelectionChanged;
        public event ISelection.SelectionChangedHandler SelectionStarted;
        public event ISelection.SelectionChangedHandler SelectionConfirmed;
        public event ISelection.SelectionChangedHandler SelectionCanceled;
        
        public CardsSelectionView(CardsSelection cardsSelection)
        {
            _cardsSelection = cardsSelection;
            _cardsSelection.SelectionStarted += OnSelectionStart;
            _cardsSelection.SelectionCanceled += OnSelectionCancel;
            _cardsSelection.SelectionConfirmed += OnSelectionConfirm;
            _cardsSelection.SelectionChanged += OnSelectionChange;
        }

        ~CardsSelectionView()
        {
            _cardsSelection.SelectionStarted -= OnSelectionStart;
            _cardsSelection.SelectionCanceled -= OnSelectionCancel;
            _cardsSelection.SelectionConfirmed -= OnSelectionConfirm;
            _cardsSelection.SelectionChanged -= OnSelectionChange;
        }

        public bool CanConfirmSelection => _cardsSelection.CanConfirmSelection;
        public bool IsActive => _cardsSelection.IsActive;
        public int CountToSelect => _cardsSelection.CountToSelect;
        public int SelectedCount => _cardsSelection.SelectedCount;

        private void OnSelectionChange(ISelection sender)
        {
            foreach (SelectedCardView selectedCardView in ToArray())
            {
                if (this.Count(x => x.ActionCardID == selectedCardView.ActionCardID) >
                       _cardsSelection.Count(x => x.ID == selectedCardView.ActionCardID))
                {
                    Remove(selectedCardView);
                }
            }
        }
        
        private void OnSelectionConfirm(ISelection sender)
        {
            Clear();
            SelectionConfirmed?.Invoke(sender);
        }
        
        private void OnSelectionStart(ISelection sender)
        {
            Clear();
            SelectionStarted?.Invoke(this);
        }
        
        private void OnSelectionCancel(ISelection sender)
        {
            Clear();
            SelectionCanceled?.Invoke(this);
        }
        
        public new void Add(SelectedCardView value)
        {
            if (Count+1 > _cardsSelection.CountToSelect)
            {
                Remove(this.First());
            }
            base.Add(value);
            
            _cardsSelection.Add(value.ActionCard);
            SelectionChanged?.Invoke(this);
        }

        public new void Remove(SelectedCardView value)
        {
            base.Remove(value);
            _cardsSelection.Remove(value.ActionCard);
            SelectionChanged?.Invoke(this);
        }
        
        public void Confirm()
        {
            _cardsSelection.Confirm();
            SelectionConfirmed?.Invoke(this);
        }

        public void Cancel()
        {
            _cardsSelection.Cancel();
            SelectionCanceled?.Invoke(this);
        }
    }
}
