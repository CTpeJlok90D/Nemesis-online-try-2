using Core.SelectionBase;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Selection
{
    public class CancelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Inject] private ISelection _selection;
        
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
            _selection.Cancel();
        }
    }
}
