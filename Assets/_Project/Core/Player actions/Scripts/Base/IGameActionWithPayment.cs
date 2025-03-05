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
}
