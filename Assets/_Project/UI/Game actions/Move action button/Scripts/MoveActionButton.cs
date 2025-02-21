using UnityEngine;
using UnityEngine.EventSystems;
using Core.PlayerActions;
using Core.Selection.Rooms;
using UI.Common;
using Zenject;

namespace UI.SelectionBase
{
    [RequireComponent(typeof(PointerEvents))]
    public class MoveActionButton : MonoBehaviour
    {
        [SerializeField] private MoveAction _moveAction;
        [Inject] private RoomSelection _roomSelection;

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

        private void OnPointerClick(PointerEvents pointerEvents, PointerEventData eventData)
        {
            _roomSelection.MaxCount = 1;
        }
    }
}
