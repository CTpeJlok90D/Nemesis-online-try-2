using System.Collections.Generic;
using Core.ActionsCards;
using Core.Selection.Cards;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace UI.Selection.Cards
{
    public class CardsSelectionSource : MonoBehaviour
    {
        [SerializeField] private ActionCardContainer _actionCardContainer_PREFAB;
        [SerializeField] private Transform _root;

        private List<ActionCardContainer> _instanses = new();

        [Inject] private CardsSelection _cardsSelection;
        
        private void Awake()
        {
            if (_root == null)
            {
                _root = transform;
            }
        }

        private void OnEnable()
        {
            _cardsSelection.SelectionStarted += OnSelectionStart;
            _cardsSelection.SelectionConfirmed += OnSelectionConfirm;
            _cardsSelection.SelectionCanceled += OnSelectionCancel;
            UpdateSelectionSource();
        }

        private void OnDisable()
        {
            _cardsSelection.SelectionStarted -= OnSelectionStart;
            _cardsSelection.SelectionConfirmed -= OnSelectionConfirm;
            _cardsSelection.SelectionCanceled -= OnSelectionCancel;
        }

        private void OnSelectionCancel(ISelection sender)
        {
            ClearInstances();
        }

        private void OnSelectionStart(ISelection sender)
        {
            UpdateSelectionSource();
        }

        private void OnSelectionConfirm(ISelection sender)
        {
            ClearInstances();
        }

        private void UpdateSelectionSource()
        {
            ClearInstances();
            InstantiateCards();
        }

        private void ClearInstances()
        {
            foreach (ActionCardContainer instance in _instanses)
            {
                Destroy(instance.gameObject);
            }
        }

        private void InstantiateCards()
        {
            foreach (ActionCard actionCard in _cardsSelection)
            {
                _actionCardContainer_PREFAB.Instantiate(actionCard, _root);
            }
        }
    }
}