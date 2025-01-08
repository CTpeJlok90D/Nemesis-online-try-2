using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;

namespace Core.PlayerActions
{
    public interface IGameActionWithPayment
    {
        public int RequredPaymentCount { get; }

        public bool CanAddPaymentToSelection(ActionCard roomCell);
    }

    public static class AdderToPaymentSelection
    {
        public static void AddToSelection(this IGameActionWithPayment gameActionWithSelection, ActionCard objectToAdd)
        {
            if (gameActionWithSelection.CanAddPaymentToSelection(objectToAdd) && PlayerActionExecutor.Instance.CanAddPaymentToSelection(objectToAdd))
            {
                PlayerActionExecutor.Instance.AddPaymentToSelection(objectToAdd);
            }
        }

        public static bool RemoveFromSelection(this IGameActionWithPayment gameActionWithSelection, ActionCard objectToRemove)
        {
            return PlayerActionExecutor.Instance.RemovePaymentFromSelection(objectToRemove);
        }

        public static int GetSelectedCount(this IGameActionWithPayment gameActionWithSelection)
        {
            return PlayerActionExecutor.Instance.PaymentCount;
        }

        public async static Task<ActionCard[]> GetSelectedPayment(this IGameActionWithPayment gameActionWithSelection)
        {
            return (await PlayerActionExecutor.Instance.GetPayment()).ToArray();
        }
    }
}
