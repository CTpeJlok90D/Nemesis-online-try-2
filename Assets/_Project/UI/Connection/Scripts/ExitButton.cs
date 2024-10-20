using UnityEngine;
using UnityEngine.UI;

namespace UI.Connection
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

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
            Application.Quit();
        }
    }
}
