using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels 
{
    public class PanelButton : MonoBehaviour
    {
        [field: SerializeField] private GameObject _panel;
        [field: SerializeField] private Button _button;

        private GameObject _instance;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (_instance == null) 
            {
                SpawnPanel();
            }
        }
        private void SpawnPanel() 
        {
            _instance = Instantiate(_panel);
        }
    }
}