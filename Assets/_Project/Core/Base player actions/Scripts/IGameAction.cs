namespace Core.BasePlayerActions
{
    public interface IGameAction
    {
        public bool CanExecute();
        public void Execute();
    }
}
