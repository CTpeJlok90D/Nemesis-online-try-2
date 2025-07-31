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
            UpdateInteractable();
            _button.onClick.AddListener(OnButtonClick);
            _selection.Changed += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            _selection.Changed -= OnSelectionChanged;
        }

        private void OnSelectionChanged(ISelection sender)
        {
            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            _button.interactable = _selection.CanConfirmSelection;
        }

        private void OnButtonClick()
        {
            _selection.Confirm();
        }
    }
}
