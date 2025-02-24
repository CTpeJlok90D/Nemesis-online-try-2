using System.Collections;

namespace Core.SelectionBase
{
    public interface ISelection
    {
        public bool CanConfirmSelection { get; }
        public int CountToSelect { get; }
        public int SelectedCount { get; }
        public void Comfirm();

        public delegate void SelectionChangedHandler(ISelection sender);
        public event SelectionChangedHandler SelectionChanged;
    }
}
