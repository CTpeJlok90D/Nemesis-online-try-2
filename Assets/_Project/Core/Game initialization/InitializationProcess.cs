namespace Core.GameInitialization
{
    public class InitializationProcess
    {
        public delegate void CompletedListener(InitializationProcess sender);

        public event CompletedListener Completed;

        internal InitializationProcess() { }

        public void Complete() 
        {
            Completed?.Invoke(this);
        }
    }
}
