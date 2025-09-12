using System;
using System.Collections.Generic;
using Core.ActionsCards;
using Core.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.Hands
{
    public class ActionCardImage : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private ActionCardContainer _actionCardContainer;
        [SerializeField] private SpriteByID _cardImages;

        private Tween _tween;

        private void OnEnable()
        {
            _ = LoadImage();
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        private async UniTask LoadImage()
        {
            _image.color = new  Color(1, 1, 1, 0);
            _image.sprite = await _cardImages.LoadAssetAsync(_actionCardContainer.ActionCard.ID);
            _image.gameObject.SetActive(true);
            _tween = _image.DOColor(Color.white, 0.5f);
        }
    }
}
