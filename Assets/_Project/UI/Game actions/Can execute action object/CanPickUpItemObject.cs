using System;
using System.Linq;
using Core.CharacterInventories;
using Core.Maps;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.GameActions
{
    [DefaultExecutionOrder(1)]
    public class CanPickUpItemObject : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        private PlayerTablet LocalTablet => PlayerTablet.Local;

        private void OnEnable()
        {
            _ = UpdateObject();
            DroppedInRoomItem.Spawned += OnItemSpawn;
            DroppedInRoomItem.Despawned += OnItemDespawn;
        }

        private void OnDisable()
        {
            DroppedInRoomItem.Spawned -= OnItemSpawn;
            DroppedInRoomItem.Despawned -= OnItemDespawn;
        }

        private void OnItemDespawn(DroppedInRoomItem spawned) => _ = UpdateObject();
        private void OnItemSpawn(DroppedInRoomItem spawned) => _ = UpdateObject();
        private async UniTask UpdateObject()
        {
            if (LocalTablet == null || LocalTablet.CharacterPawn == null)
            {
                _target.SetActive(false);
                return;
            }

            await UniTask.NextFrame();
            RoomCell cell = LocalTablet.CharacterPawn.RoomContent.Owner;
            _target.SetActive(cell.RoomContents.Any(x => x.TryGetComponent(out DroppedInRoomItem item)));
        }
    }
}