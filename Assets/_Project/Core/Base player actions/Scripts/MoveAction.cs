using System;
using Core.ActionsCards;
using Core.Maps;
using UnityEngine;

namespace Core.BasePlayerActions
{
    public class MoveAction : ScriptableObject, IGameAction, IGameActionWithPayment, IGameActionWithRoomsSelection, INeedMap
    {
        [field: SerializeField] public int RequredRoomsCount { get; private set; }

        [field: SerializeField] public int RequredPaymentCount { get; private set; }

        private Map _map;

        public bool CanAddPaymentToSelection(ActionCard paymentCard)
        {
            return true;
        }

        public bool CanAddRoomToSelection(RoomCell roomCell)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            
        }

        public void Init(Map map)
        {
            _map = map;
        }
    }
}