using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.Common
{
    public class ImmidiatlyTMPLocalization : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private LocalizedString _text;

        private void OnEnable()
        {
            ValidateString();
            _text.StringChanged += OnStringChange;   
        }

        private void OnDisable()
        {
            _text.StringChanged -= OnStringChange;
        }

        private void OnStringChange(string value)
        {
            ValidateString();
        }

        private void ValidateString()
        {
            _label.text = _text.GetLocalizedString();
        }
    }
}
