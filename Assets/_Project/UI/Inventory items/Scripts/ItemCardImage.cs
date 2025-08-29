using Core.CharacterInventories;
using Core.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OdinSerializer;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hands
{
    public class ItemCardImage : SerializedMonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SerializableInterface<IContainsInventoryItemInstance> _inventoryItemInstance;
        [SerializeField] private SpriteByID _cardImages;
        
        private IContainsInventoryItemInstance InventoryItem => _inventoryItemInstance?.Value;

        private void OnEnable()
        {
            _ = LoadSprite();
        }

        private async UniTask LoadSprite()
        {
            Color oldColor = _image.color;
            _image.color = new Color(oldColor.r,oldColor.g,oldColor.b,0);
            Sprite loadedSprite = await _cardImages.LoadAsset(InventoryItem.ID);
            _image.sprite = loadedSprite;
            _image.DOColor(oldColor, 0.2f);
        }
    }
}
