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
        
        public event ISelection.SelectionChangedHandler Changed;
        public event ISelection.SelectionChangedHandler Started;
        public event ISelection.SelectionChangedHandler Confirmed;
        public event ISelection.SelectionChangedHandler Canceled;
        
        public CardsSelectionView(CardsSelection cardsSelection)
        {
            _cardsSelection = cardsSelection;
            _cardsSelection.Started += OnStart;
            _cardsSelection.Canceled += OnCancel;
            _cardsSelection.Confirmed += OnConfirm;
            _cardsSelection.Changed += OnChange;
        }

        ~CardsSelectionView()
        {
            _cardsSelection.Started -= OnStart;
            _cardsSelection.Canceled -= OnCancel;
            _cardsSelection.Confirmed -= OnConfirm;
            _cardsSelection.Changed -= OnChange;
        }

        public bool CanConfirmSelection => _cardsSelection.CanConfirmSelection;
        public bool IsActive => _cardsSelection.IsActive;
        public int RequiredCount => _cardsSelection.RequiredCount;
        public int SelectedCount => _cardsSelection.SelectedCount;
        public bool CanCancel => true;

        private void OnChange(ISelection sender)
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
        
        private void OnConfirm(ISelection sender)
        {
            Clear();
            Confirmed?.Invoke(sender);
        }
        
        private void OnStart(ISelection sender)
        {
            Clear();
            Started?.Invoke(this);
        }
        
        private void OnCancel(ISelection sender)
        {
            Clear();
            Canceled?.Invoke(this);
        }
        
        public new void Add(SelectedCardView value)
        {
            if (Count+1 > _cardsSelection.RequiredCount)
            {
                Remove(this.First());
            }
            base.Add(value);
            
            _cardsSelection.Add(value.ActionCard);
            Changed?.Invoke(this);
        }

        public new void Remove(SelectedCardView value)
        {
            base.Remove(value);
            _cardsSelection.Remove(value.ActionCard);
            Changed?.Invoke(this);
        }
        
        public void Confirm()
        {
            _cardsSelection.Confirm();
            Confirmed?.Invoke(this);
        }

        public void Cancel()
        {
            _cardsSelection.Cancel();
            Canceled?.Invoke(this);
        }
    }
}
