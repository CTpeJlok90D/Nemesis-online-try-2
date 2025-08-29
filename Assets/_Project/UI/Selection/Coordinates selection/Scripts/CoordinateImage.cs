using System.Collections.Generic;
using Core;
using Core.Common;
using Core.DestinationCoordinats;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI
{
    public class CoordinateImage : MonoBehaviour
    {
        [SerializeField] private CoordinateContainer _coordinateContainer;
        [SerializeField] private Image _image;
        [SerializeField] private SpriteByID _coordinateCards;

        private Coordinate _coordinate;
        private Dictionary<string, AsyncOperationHandle<Sprite>> _spriteLoadHandles = new();

        private void Update()
        {
            UpdateAvatar();
        }

        private void UpdateAvatar()
        {
            if (_coordinate == _coordinateContainer.Coordinate.Value)
            {
                return;
            }

            _coordinate = _coordinateContainer.Coordinate.Value;

            if (_coordinate == null || string.IsNullOrEmpty(_coordinate.Id))
            {
                _image.enabled = false;
                return;
            }

            if (_image.enabled == false)
            {
                _image.enabled = true;
            }

            if (_spriteLoadHandles.TryGetValue(_coordinate.Id, out AsyncOperationHandle<Sprite> handle))
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

            AssetReferenceT<Sprite> avatarReference = _coordinateCards[_coordinate.Id];
            AsyncOperationHandle<Sprite> assetReferenceHandle = Addressables.LoadAssetAsync<Sprite>(avatarReference.RuntimeKey);
            assetReferenceHandle.Completed += OnHandleLoad;

            _spriteLoadHandles.Add(_coordinate.Id, assetReferenceHandle);
            _image.color = new Color(1,1,1,0);
        }

        private void OnHandleLoad(AsyncOperationHandle<Sprite> handle)
        {
            if (_image == null)
            {
                return;
            }
            _image.sprite = handle.Result;
            _image.DOColor(Color.white, 0.5f);
        }
    }
}