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
            _selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _selection.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(ISelection sender)
        {
            _caption.text = string.Format(_format, sender.SelectedCount, sender.CountToSelect);
        }
    }
}
