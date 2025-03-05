using TMPro;
using UnityEngine;
using Zenject;

namespace Core.SelectionBase
{
    public class SelectionCount : MonoBehaviour
    {
        [SerializeField] private TMP_Text _caption;

        [Inject] private ISelection _selection;
        private string _format = "{0}/{1}";

        private void OnEnable()
        {
            _selection.SelectionChanged += UpdateText;
            _selection.SelectionConfirmed += UpdateText;
            _selection.SelectionCanceled += UpdateText;
        }

        private void OnDisable()
        {
            _selection.SelectionChanged -= UpdateText;
            _selection.SelectionConfirmed += UpdateText;
            _selection.SelectionCanceled += UpdateText;
        }

        private void UpdateText(ISelection sender) => UpdateText();
        private void UpdateText()
        {
            _caption.text = string.Format(_format, _selection.SelectedCount, _selection.CountToSelect);
        }
    }
}
