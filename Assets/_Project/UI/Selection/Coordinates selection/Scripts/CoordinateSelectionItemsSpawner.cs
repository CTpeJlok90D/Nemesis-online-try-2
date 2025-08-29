using System.Collections.Generic;
using Core;
using Core.DestinationCoordinats;
using Core.SelectionBase;
using UI.Common;
using UI.Selection;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(PointerEvents))]
    public class CoordinateSelectionItemsSpawner : MonoBehaviour
    {
        [SerializeField] private CoordinateContainer _coordinateContainer_PREFAB;
        [SerializeField] private Transform _coordinateParent;

        [Inject] private CoordinatesSelection _selection;
        private List<CoordinateContainer> _coordinateContainers = new();
        
        private void OnEnable()
        {
            UpdateInstances();
            _selection.Started += OnSelectionStart;
            _selection.Confirmed += OnSelectionConfirm;
            _selection.Canceled += OnSelectionCancel;
        }

        private void OnDisable()
        {
            _selection.Started -= OnSelectionStart;
            _selection.Confirmed -= OnSelectionConfirm;
            _selection.Canceled -= OnSelectionCancel;
        }

        private void OnSelectionCancel(ISelection sender) => UpdateInstances();
        private void OnSelectionConfirm(ISelection sender) => UpdateInstances();
        private void OnSelectionStart(ISelection sender) => UpdateInstances();
        private void UpdateInstances()
        {
            DestroyCards();
            InstantiateCards();
        }

        private void InstantiateCards()
        {
            foreach (Coordinate coordinate in _selection.SelectionSource)
            {
                CoordinateContainer coordinateContainer = _coordinateContainer_PREFAB.Instantiate(coordinate, _coordinateParent);
                _coordinateContainers.Add(coordinateContainer);
            }
        }

        private void DestroyCards()
        {
            foreach (CoordinateContainer container in _coordinateContainers)
            {
                Destroy(container.gameObject);
            }
            _coordinateContainers.Clear();
        }
    }
}