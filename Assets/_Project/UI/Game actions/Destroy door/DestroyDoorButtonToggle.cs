using System;
using System.Linq;
using Core;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerActions;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace UI
{
    public class DestroyDoorButtonToggle : MonoBehaviour
    {
        [SerializeField] private GameObject _toggle;

        [Inject] private Ship _ship;
        private PlayerTablet LocalTablet => PlayerTablet.Local;
        private CharacterPawn LocalPawn => LocalTablet.CharacterPawn;
        private RoomContent LocalRoomContent => LocalPawn.RoomContent;

        private void Update()
        {
            UpdateButtonActive();
        }

        private void UpdateButtonActive()
        {
            if (LocalTablet == null || LocalPawn == null)
            {
                _toggle.gameObject.SetActive(false);
                return;
            }
            
            RoomContent content = LocalTablet.CharacterPawn.RoomContent;
            RoomCell cell = content.Owner;
            bool haveAcceptableDoor = cell.Tunnels
                .Any(noiseContainer => noiseContainer.NetworkObject.TryGetComponent(out Tunnel tunnel) &&
                                       tunnel.DoorState is not DoorState.Broken);
            bool haveCard = LocalTablet.ActionCardsDeck.HandLocal.Any(card => card.ID == DestroyDoor.CardID);
            
            _toggle.SetActive(haveCard && haveAcceptableDoor);
        }
    }
}