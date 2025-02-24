using Core.SelectionBase;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Selection
{
    public class ConfirmButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Inject] private ISelection _selection;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
            _selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            _selection.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(ISelection sender)
        {
            _button.interactable = _selection.CanConfirmSelection;
        }

        private void OnButtonClick()
        {
            _selection.Comfirm();
        }
    }
}
