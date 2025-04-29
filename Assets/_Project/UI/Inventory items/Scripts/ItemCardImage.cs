using System.Collections.Generic;
using Core.CharacterInventorys;
using Core.Common;
using OdinSerializer;
using TNRD;
using UI.Selection.InventoryItemsSelections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.Hands
{
    public class ItemCardImage : SerializedMonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SerializableInterface<IContainsInventoryItemInstance> _inventoryItemInstance;
        [SerializeField] private SpriteByID _cardImages;

        private Dictionary<object, AsyncOperationHandle<Sprite>> _loadHandles = new();
        
        private IContainsInventoryItemInstance InventoryItem => _inventoryItemInstance?.Value;

        private void OnEnable()
        {
            AsyncOperationHandle<Sprite> spriteLoadHandle;
            object runtimeKey = _cardImages[InventoryItem.ID].RuntimeKey;
            if (_loadHandles.ContainsKey(runtimeKey))
            {
                spriteLoadHandle = _loadHandles[runtimeKey];
            }
            else
            {
                AssetReferenceT<Sprite> spriteReference = _cardImages[InventoryItem.ID];
                if (spriteReference.OperationHandle.IsValid() == false)
                {
                    spriteLoadHandle = spriteReference.LoadAssetAsync();
                }
                else
                {
                    spriteLoadHandle = spriteReference.OperationHandle.Convert<Sprite>();
                }
                _loadHandles.Add(runtimeKey, spriteLoadHandle);
            }

            if (spriteLoadHandle.IsDone)
            {
                _image.sprite = spriteLoadHandle.Result;
                return;
            }

            spriteLoadHandle.Completed += OnSpriteLoadComplete;
            if (gameObject != _image.gameObject)
            {
                _image.gameObject.SetActive(false);
            }
        }

        private void OnSpriteLoadComplete(AsyncOperationHandle<Sprite> handle)
        {
            _image.sprite = handle.Result;
            _image.gameObject.SetActive(true);
        }
    }
}
