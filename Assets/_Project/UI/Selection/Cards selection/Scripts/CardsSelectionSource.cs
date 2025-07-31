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

        [Inject] private DiContainer _diContainer;
        
        private void Awake()
        {
            if (_root == null)
            {
                _root = transform;
            }
        }

        private void OnEnable()
        {
            UpdateSelectionSource();
            _cardsSelection.Started += OnStart;
            _cardsSelection.Confirmed += OnConfirm;
            _cardsSelection.Canceled += OnCancel;
        }

        private void OnDisable()
        {
            _cardsSelection.Started -= OnStart;
            _cardsSelection.Confirmed -= OnConfirm;
            _cardsSelection.Canceled -= OnCancel;
        }

        private void OnCancel(ISelection sender)
        {
            ClearInstances();
        }

        private void OnStart(ISelection sender)
        {
            UpdateSelectionSource();
        }

        private void OnConfirm(ISelection sender)
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
            _instanses.Clear();
        }

        private void InstantiateCards()
        {
            foreach (ActionCard actionCard in _cardsSelection.SelectionSource)
            {
                ActionCardContainer instance = _actionCardContainer_PREFAB.Instantiate(actionCard, _diContainer, _root);
                _instanses.Add(instance);
            }
        }
    }
}