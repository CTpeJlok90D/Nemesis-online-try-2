using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.PlayerActions
{
    [CreateAssetMenu(menuName = "Game/Actions/Move action")]
    public class MoveAction : ScriptableObject, IGameAction, IGameActionWithPayment, IGameActionWithRoomsSelection, INeedMap
    {
        [field: SerializeField] public int RequredPaymentCount { get; private set; } = 1;
        public int RequredRoomsCount { get; private set; } = 1;

        private Map _map;
        private PlayerTablet _executer;
        public RoomCell RoomWithExecuter 
        {
            get
            {
                return _map.First(x => x.RoomContents.Contains(_executer.CharacterPawn.RoomContent));
            }
        } 
        public RoomCell[] Selection { get; set; }

        public RoomCell[] SelectionSource
        {
            get
            {
                IEnumerable<RoomCell> possibleRooms = GetPossibleRooms();
                return possibleRooms.ToArray();
            }  
        } 

        public bool CanAddPaymentToSelection(ActionCard paymentCard)
        {
            return true;
        }

        public bool CanAddRoomToSelection(RoomCell roomCell)
        {
            INoiseContainer[] containers = RoomWithExecuter.GetPassagesTo(roomCell);
            return containers.Length != 0;
        }

        public IEnumerable<RoomCell> GetPossibleRooms()
        {
            RoomCell roomWithExecuter = RoomWithExecuter;
            IEnumerable<RoomCell> result = from x in _map where x.GetPassagesTo(roomWithExecuter).Length != 0 select x;
            return result;
        }

        public IGameAction.CanExecuteCheckResult CanExecute()
        {
            bool boolResult;
            RoomCell[] selectedRooms = Selection; 

            if (_executer.ActionCount.Value <= 0)
            {
                boolResult = false;
                IGameAction.CanExecuteCheckResult result = new()
                {
                    Result = false,
                    Error = new InvalidOperationException($"Not enoth action points to execute action"),
                };

                return result;
            }

            if (selectedRooms.Length != 1)
            {
                boolResult = false;
                IGameAction.CanExecuteCheckResult result = new()
                {
                    Result = false,
                    Error = new InvalidOperationException($"To execute {nameof(MoveAction)}, room selection must contains only 1 room"),
                };

                return result;
            }

            RoomCell selectedRoom = selectedRooms.First();

            IEnumerable<RoomCell> possibleRooms = GetPossibleRooms();
            
            boolResult = possibleRooms.Contains(selectedRoom);
            return new()
            {
                Result = boolResult,
                Error = boolResult ? null : new InvalidOperationException($"No path to move in selected room"),
            };
        }

        public void Execute()
        {
            IGameAction.CanExecuteCheckResult chekResult = CanExecute();
            if (chekResult == false)
            {
                throw chekResult.Error;
            }

            ForceExecute();
        }

        public void ForceExecute()
        {
            RoomCell selectedRoom = Selection.First();

            _executer.ActionCount.Value--;
            selectedRoom.AddContent(_executer.CharacterPawn.RoomContent);
        }

        public void Initialzie(Map map)
        {
            _map = map;
        }

        public void Inititalize(PlayerTablet executer)
        {
            _executer = executer;
        }
    }
}