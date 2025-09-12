using System;
using UI.CommonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TabImageSwapper : MonoBehaviour
    {
        [SerializeField] private Tab _tab;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _enabledSprite;
        [SerializeField] private Sprite _disabledSprite;

        private void OnEnable()
        {
            UpdateSprite();
            _tab.Enabled += OnTabEnable;
            _tab.Disabled += OnTabDisable;
        }

        private void OnDisable()
        {
            _tab.Enabled -= OnTabEnable;
            _tab.Disabled -= OnTabDisable;
        }

        private void OnTabDisable() => UpdateSprite();
        private void OnTabEnable() => UpdateSprite();
        private void UpdateSprite()
        {
            _image.sprite = _tab.gameObject.activeSelf ? _enabledSprite : _disabledSprite;
        }
    }
}