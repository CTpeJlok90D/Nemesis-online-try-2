using Core.ActionsCards;
using Core.PlayerActions;

namespace UI.SelectionBase
{
    public class ActionCardsSelection : Selection<ActionCard>
    {
        public ActionCardsSelection(int maxCount) : base(maxCount)
        {
            
        }

        public override void Add(ActionCard value)
        {
            base.Add(value);
            PlayerActionExecutor.Instance.AddPaymentToSelection(value);
        }

        public override void Remove(ActionCard value)
        {
            base.Remove(value);
            PlayerActionExecutor.Instance.RemovePaymentFromSelection(value);
        }
    }
}
