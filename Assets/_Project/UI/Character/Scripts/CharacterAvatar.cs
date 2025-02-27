using System.Collections.Generic;
using AYellowpaper;
using Core.Characters;
using Core.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.Characters
{
    public class CharacterAvatar : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IContainsCharacter> _characterContainer;
        [SerializeField] private Image _image;        
        [SerializeField] private SpriteByID _avatars;

        private Character _character;
        private Dictionary<string, AsyncOperationHandle<Sprite>> _avatarLoadHandles = new();

        private void Update()
        {
            UpdateAvatar();
        }

        private void UpdateAvatar()
        {
            if (_character == _characterContainer.Value.Character)
            {
                return;
            }

            _character = _characterContainer.Value.Character;

            if (_character == null || string.IsNullOrEmpty(_character.Id))
            {
                _image.enabled = false;
                return;
            }

            if (_image.enabled == false)
            {
                _image.enabled = true;
            }

            if (_avatarLoadHandles.TryGetValue(_character.Id, out AsyncOperationHandle<Sprite> handle))
            {
                if (handle.IsDone)
                {
                    _image.sprite = handle.Result;
                }
                else
                {
                    handle.Completed += OnHandleLoad;
                }
                return;
            }

            AssetReferenceT<Sprite> avatarReference = _avatars[_character.Id];
            AsyncOperationHandle<Sprite> assetReferenceHandle = avatarReference.LoadAssetAsync();
            assetReferenceHandle.Completed += OnHandleLoad;

            _avatarLoadHandles.Add(_character.Id, assetReferenceHandle);
            _image.color = new Color(1,1,1,0);

            
        }

        private void OnHandleLoad(AsyncOperationHandle<Sprite> handle)
        {
            _image.sprite = handle.Result;
            _image.DOColor(Color.white, 0.5f);
        }
    }
}
