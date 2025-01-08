using System;
using Core.PlayerTablets;

namespace Core.PlayerActions
{
    public interface IGameAction
    {
        public void Init(PlayerTablet executor);
        public CanExecuteCheckResult CanExecute();
        public void Execute();

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
