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
    [DefaultExecutionOrder(1)]
    public class ItemCardImage : SerializedMonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SerializableInterface<IContainsInventoryItemInstance> _inventoryItemInstance;
        [SerializeField] private SpriteByID _cardImages;
        [SerializeField] private Sprite _nullSprite;

        private IContainsInventoryItemInstance ItemContainer => _inventoryItemInstance?.Value;
        public InventoryItem Item => ItemContainer?.Item;

        private void OnEnable()
        {
            _ = LoadSprite();
        }

        private async UniTask LoadSprite()
        {
            Color oldColor = _image.color;
            _image.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0);

            if (Item == null)
            {
                _image.sprite = _nullSprite;
                _image.DOColor(oldColor, 0.2f);
                return;
            }

            Sprite loadedSprite = await _cardImages.LoadAsset(Item.ID);
            _image.sprite = loadedSprite;
            _image.DOColor(oldColor, 0.2f);
        }
    }
}
