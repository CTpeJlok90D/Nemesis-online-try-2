using System.Collections;

namespace Core.SelectionBase
{
    public interface ISelection
    {
        public bool CanConfirmSelection { get; }
        public bool IsActive { get; }
        public int RequiredCount { get; }
        public int SelectedCount { get; }
        public bool CanCancel { get; }
        public void Confirm();
        public void Cancel();

        public delegate void SelectionChangedHandler(ISelection sender);
        public event SelectionChangedHandler Changed;
        public event SelectionChangedHandler Started;
        public event SelectionChangedHandler Confirmed;
        public event SelectionChangedHandler Canceled;
        
    }
}
