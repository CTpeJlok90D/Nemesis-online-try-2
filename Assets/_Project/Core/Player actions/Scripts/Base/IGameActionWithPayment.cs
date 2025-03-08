using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;

namespace Core.PlayerActions
{
    public interface IGameActionWithPayment
    {
        public int RequaredPaymentCount { get; }
    }
}
