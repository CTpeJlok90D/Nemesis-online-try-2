using System.Linq;
using Core;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerTablets;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.GameActions
{
    [RequireComponent(typeof(PointerEvents))]
    public class RoomActionButton : MonoBehaviour
    {
        [SerializeField] private IconByRoomID _roomIcon;
        public ReactiveField<int> RoomActionIndex { get; private set; } = new();
        
        private PointerEvents _pointerEvents;
        
        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }
        
        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnButtonClick;
            RoomActionIndex.Changed += OnRoomActionIndexChange;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnButtonClick;
            RoomActionIndex.Changed -= OnRoomActionIndexChange;
        }

        private void OnRoomActionIndexChange(int oldValue, int newValue)
        {
            _roomIcon.RoomActionIndex.Value = newValue;
        }

        private void OnButtonClick(PointerEvents pointerEvents, PointerEventData eventData)
        {
            OnButtonClick();
        }

        private void OnButtonClick()
        {
            RoomCell roomCell = PlayerTablet.Local.CharacterPawn.RoomContent.Owner;
            PlayerActionExecutor.Singleton.Execute(roomCell.Type.RoomActions.First());
        }
    }
}