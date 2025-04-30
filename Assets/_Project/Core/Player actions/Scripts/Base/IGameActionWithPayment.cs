using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerTablets;
using Core.Selection.Cards;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.PlayerActions
{
    public interface IGameActionWithPayment
    {
        public int RequaredPaymentCount { get; }

        internal async UniTask<ActionCard[]> GetSelectionLocal(PlayerTablet executor, CardsSelection cardsSelection)
        {
            int requiredPaymentCount = RequaredPaymentCount;
            IReadOnlyCollection<ActionCard> hand = await executor.ActionCardsDeck.GetHand();
            hand = hand.Where(x => x.Type == ActionCard.InfectionType.Basic).ToList();

            ActionCard[] selectedCards = await cardsSelection.SelectFrom(hand, requiredPaymentCount);
                    
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
