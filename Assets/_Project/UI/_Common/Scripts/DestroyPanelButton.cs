using UnityEngine;
using UnityEngine.UI;

namespace UI.CommonScripts
{
    public class DestroyPanelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _Panel;

        private void OnEnable()
        {
            _button.onClick.AddListener(DestroyPanel);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(DestroyPanel);
        }

        public void DestroyPanel()
        {
            Destroy(_Panel);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_button == null) 
            {
                _button = GetComponent<Button>();
            }
        }
#endif
    }
}
