using System;
using System.Linq;
using Core.ActionsCards;
using Core.Selection.Cards;
using UnityEngine;
using Zenject;

namespace SelectionStarted
{
    public class SelectedCardView : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private ActionCardContainer _actionCardContainer;

        [Inject] private CardsSelection _cardsSelection;

        private void Update()
        {
            _target.SetActive(_cardsSelection.Contains(_actionCardContainer.ActionCard));
        }
    }
}
