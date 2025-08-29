using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.Maps;
using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class AttackButtonToggle : MonoBehaviour
    {
        [SerializeField] private GameObject _button;
        
        private void Update()
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (PlayerTablet.Local == null || PlayerTablet.LocalCharacterPawn == null || PlayerTablet.LocalRoomCell == null)
            {
                _button.SetActive(false);
                return;
            }

            IReadOnlyCollection<RoomContent> contents = PlayerTablet.LocalRoomCell.RoomContents;
            _button.SetActive(contents.Any(x => x.TryGetComponent(out Enemy e)));
        }
    }
}