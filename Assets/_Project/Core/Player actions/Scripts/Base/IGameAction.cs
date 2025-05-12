using System;
using Core.PlayerTablets;

namespace Core.PlayerActions
{
    public interface IGameAction
    {
        public bool CanCancel => true;
        public void Inititalize(PlayerTablet executor);
        public CanExecuteCheckResult CanExecute();
        public void Execute();
        public void ForceExecute();


        public struct CanExecuteCheckResult
        {
            public static implicit operator bool(CanExecuteCheckResult obj)
            {
                return obj.Result;
            }
            
            public bool Result;
            public Exception Error;
        }
    }
}
