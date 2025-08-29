using Core;
using Core.SelectionBase;
using UI.Common;
using UI.Selection;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(PointerEvents))]
    public class CoordinateSelectionItem : MonoBehaviour
    {
        [SerializeField] private CoordinateContainer _coordinateContainer;
        [SerializeField] private GameObject _isSelectedObject;
        
        [Inject] private CoordinatesSelection _selection;
        private PointerEvents _pointerEvents;
        
        private void OnEnable()
        {
            _selection.Changed += OnSelectionChange;
            _pointerEvents.PointerClicked += OnPointerClicked;
        }

        private void OnDisable()
        {
            _selection.Changed -= OnSelectionChange;
            _pointerEvents.PointerClicked -= OnPointerClicked;
        }

        private void OnPointerClicked(PointerEvents sender, PointerEventData eventData)
        {
            _selection.Add(_coordinateContainer.Coordinate.Value);
        }

        private void OnSelectionChange(ISelection sender)
        {
            _isSelectedObject.SetActive(_selection.IsSelected(_coordinateContainer.Coordinate.Value));
        }
    }
}