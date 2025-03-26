using System.Collections;

namespace Core.SelectionBase
{
    public interface ISelection
    {
        public bool CanConfirmSelection { get; }
        public bool IsActive { get; }
        public int CountToSelect { get; }
        public int SelectedCount { get; }
        public void Confirm();
        public void Cancel();

        public delegate void SelectionChangedHandler(ISelection sender);
        public event SelectionChangedHandler SelectionChanged;
        public event SelectionChangedHandler SelectionStarted;
        public event SelectionChangedHandler SelectionConfirmed;
        public event SelectionChangedHandler SelectionCanceled;
    }
}
