using System;
using System.Collections.Generic;
using Core.Common;
using Core.Missions;
using Cysharp.Threading.Tasks;
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
        
        private Tween _tween; 
            
        private void OnEnable()
        {
            _ = UpdateSprite();
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        private async UniTask UpdateSprite()
        {
            if (Mission == null)
            {
                return;
            }

            _image.color = new Color(1,1,1,0);
            _image.sprite = await _missions.LoadAssetAsync(_mission.Value.Mission.ID);
            _tween = _image.DOColor(Color.white, 0.5f);
        }
    }
}