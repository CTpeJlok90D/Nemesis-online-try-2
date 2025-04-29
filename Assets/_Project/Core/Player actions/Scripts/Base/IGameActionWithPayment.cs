using System;
using System.Collections.Generic;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerTablets;
using Core.Selection.Cards;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Custom;

namespace Core.PlayerActions
{
    public interface IGameActionWithPayment
    {
        public int RequaredPaymentCount { get; }

        internal async UniTask<ActionCard[]> GetSelectionLocal(PlayerTablet executor, CardsSelection cardsSelection, NetVariable<bool> actionIsExecuting)
        {
            int requiredPaymentCount = RequaredPaymentCount;
            IReadOnlyCollection<ActionCard> hand = await executor.ActionCardsDeck.GetHand();

            ActionCard[] selectedCards = await cardsSelection.SelectFrom(hand, requiredPaymentCount);

            if (selectedCards.Length != requiredPaymentCount)
            {
                actionIsExecuting.Value = false;
                return Array.Empty<ActionCard>();
            }
                    
            return selectedCards;
        }
    }

    public interface INeedNoiseContainers
    {
        public INoiseContainer[] SelectedNoiseContainers { get; set; }

        public INoiseContainer[] NoiseContainerSelectionSource { get; }
        
        public int RequiredNoiseContainerCount { get; }
    }
}
