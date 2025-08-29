using System.Collections.Generic;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using UI.GameActions;
using UnityEngine;

namespace UI
{
    public class RoomActionsButtonsSpawner : MonoBehaviour
    {
        [SerializeField] private RoomActionButton _roomActionButton_PREFAB;
        [SerializeField] private Transform _buttonsParent;
        
        private List<RoomActionButton> _roomActionButtonInstances = new();
        
        private void OnEnable()
        {
            PlayerTablet.Local.CharacterPawnReference.Changed += OnCharacterPawnChange;
            if (PlayerTablet.LocalCharacterPawn != null)
            {
                PlayerTablet.LocalRoomContent.OwnerChanged += OnOwnerChange;
            }
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            {
                PlayerTablet.Local.CharacterPawnReference.Changed -= OnCharacterPawnChange;
            }
            if (PlayerTablet.Local != null && PlayerTablet.Local.CharacterPawn != null)
            {
                PlayerTablet.LocalRoomContent.OwnerChanged -= OnOwnerChange;
            }
        }

        private void OnCharacterPawnChange(CharacterPawn oldValue, CharacterPawn newValue)
        {
            if (oldValue != null)
            {
                oldValue.RoomContent.OwnerChanged -= OnOwnerChange;
            }

            if (newValue != null)
            {
                newValue.RoomContent.OwnerChanged += OnOwnerChange;
            }
        }

        private void OnOwnerChange(RoomCell oldValue, RoomCell newValue)
        {
            for (int index = 0; index < newValue.Type.RoomActions.Length; index++)
            {
                if (_roomActionButtonInstances.Count <= index)
                {
                    _roomActionButtonInstances.Add(Instantiate(_roomActionButton_PREFAB, _buttonsParent));
                }

                bool buttonIsActive = index < newValue.Type.RoomActions.Length
                    && newValue.Type.RoomActions[index].GameAction.Value.CanExecute();
                
                RoomActionButton roomActionButton = _roomActionButtonInstances[index];
                roomActionButton.gameObject.SetActive(buttonIsActive);
                roomActionButton.RoomActionIndex.Value = index;
            }
        }
    }
}