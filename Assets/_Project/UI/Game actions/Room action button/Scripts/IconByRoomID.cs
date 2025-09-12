using System.Linq;
using Core;
using Core.Common;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class IconByRoomID : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SpriteByID _icons;

        public ReactiveField<int> RoomActionIndex { get; private set; } = new();
        
        private void OnEnable()
        {
            PlayerTablet.Local.CharacterPawn.RoomContent.OwnerChanged += OnOwnerChange;
            RoomActionIndex.Changed += OnRoomActionIndexChange;
            _ = UpdateRoomIcon();
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null && PlayerTablet.Local.CharacterPawn != null)
            {
                PlayerTablet.Local.CharacterPawn.RoomContent.OwnerChanged -= OnOwnerChange;
            }
            RoomActionIndex.Changed -= OnRoomActionIndexChange;
        }

        private void OnRoomActionIndexChange(int oldValue, int newValue) => _ = UpdateRoomIcon();
        private void OnOwnerChange(RoomCell oldValue, RoomCell newValue) => _ = UpdateRoomIcon();
        private async UniTask UpdateRoomIcon()
        {
            GameActionContainer gameActionContainer = PlayerTablet.LocalRoomCell.Type.RoomActions.First();
            
            _image.sprite = null;
            _image.sprite = await _icons.LoadAssetAsync(gameActionContainer.Id);
        }
    }
}