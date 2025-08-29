using System.Collections.Generic;
using Core.Common;
using Core.Missions;
using DG.Tweening;
using TNRD;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI
{
    public class MissionSprite : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IContainsMission> _mission;
        [SerializeField] private Image _image;
        [SerializeField] private SpriteByID _missions;

        public Mission Mission => _mission.Value.Mission;

        private Dictionary<string, AsyncOperationHandle<Sprite>> _missionsLoadingTasks = new(); 
        
        private void Start()
        {
            UpdateSprite();
        }

        private void OnEnable()
        {
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (Mission == null)
            {
                return;
            }

            if (_missionsLoadingTasks.TryGetValue(Mission.ID, out AsyncOperationHandle<Sprite> handle))
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

            AssetReferenceT<Sprite> avatarReference = _missions[Mission.ID];
            AsyncOperationHandle<Sprite> assetReferenceHandle = Addressables.LoadAssetAsync<Sprite>(avatarReference.RuntimeKey);
            assetReferenceHandle.Completed += OnHandleLoad;

            _missionsLoadingTasks.Add(Mission.ID, assetReferenceHandle);
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