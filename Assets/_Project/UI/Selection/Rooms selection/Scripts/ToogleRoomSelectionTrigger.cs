using System;
using System.Linq;
using Core.Maps;
using Core.Selection.Rooms;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Selection.Rooms
{
    [RequireComponent(typeof(PointerEvents))]
    public class ToogleRoomSelectionTrigger : MonoBehaviour
    {
        [SerializeField] private RoomCell _roomCell;

        [Inject] private RoomsSelection _roomSelection;

        private PointerEvents _pointerEvents;


        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnClick;
        }

        private void OnClick(PointerEvents pointerEvents, PointerEventData eventData)
        {
            if (_roomSelection.CanSelect(_roomCell) == false)
            {
                return;
            }
            
            if (_roomSelection.Contains(_roomCell))
            {
                _roomSelection.Remove(_roomCell);
            }
            else
            {
                _roomSelection.Add(_roomCell);
            }
        }
    }
}