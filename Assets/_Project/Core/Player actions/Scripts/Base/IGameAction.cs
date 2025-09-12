using System;
using Core.PlayerTablets;

namespace Core.PlayerActions
{
    public interface IGameAction
    {
        public bool SuitableCondictionsForFulfillment(PlayerTablet executor);
        public bool CanCancel => true;
        public void Initialize(PlayerTablet executor);
        public CanExecuteCheckResult CanExecute();

        public virtual void Execute()
        {
            CanExecuteCheckResult checkResult = CanExecute();
            if (checkResult == false)
            {
                throw checkResult.Error;
            }

            ForceExecute();
        }
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
