using System;
using System.Linq;
using Core.Maps;
using Core.Selection.RoomContentSelections;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Selection.RoomConeneSelection
{
    [RequireComponent(typeof(PointerEvents))]
    public class ToogleRoomContentSelectionTrigger : MonoBehaviour
    {
        [SerializeField] private RoomContent _roomContent;

        [Inject] private RoomContentSelection _roomSelection;

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
            if (_roomSelection.CanSelect(_roomContent) == false)
            {
                return;
            }
            
            if (_roomSelection.Contains(_roomContent))
            {
                _roomSelection.Remove(_roomContent);
            }
            else
            {
                _roomSelection.Add(_roomContent);
            }
        }
    }
}