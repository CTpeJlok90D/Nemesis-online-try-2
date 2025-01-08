using System.Collections.Generic;
using Core.ActionsCards;
using Core.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

namespace UI.Hands
{
    public class ActionCardImage : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private ActionCardContainer _actionCardContainer;
        [SerializeField] private SpriteByID _cardImages;

        private Dictionary<object, AsyncOperationHandle<Sprite>> _loadHandles = new();

        private void OnEnable()
        {
            AsyncOperationHandle<Sprite> spriteLoadHandle;
            object runtimeKey = _cardImages[_actionCardContainer.ActionCard.ID].RuntimeKey;
            if (_loadHandles.ContainsKey(runtimeKey))
            {
                spriteLoadHandle = _loadHandles[runtimeKey];
            }
            else
            {
                AssetReferenceT<Sprite> spriteReference = _cardImages[_actionCardContainer.ActionCard.ID];
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
