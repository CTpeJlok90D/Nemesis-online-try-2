using System;
using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.PlayerActions
{
    [CreateAssetMenu(menuName = Constants.ACTIONS_CREATE_PARH + "Move action")]
    public class MoveAction : ScriptableObject, IGameAction, INeedPayment, INeedRooms, INeedMap
    {
        public Map Map { get; private set; }
        public PlayerTablet Executor { get; private set; }
        public RoomCell[] RoomSelection { get; set; }
        public virtual int RequaredPaymentCount => 1;
        public int RequredRoomsCount => 1;
        public RoomCell RoomWithExecutor => Map.First(x => x.RoomContents.Contains(Executor.CharacterPawn.RoomContent));
        public RoomCell[] RoomSelectionSource => GetPossibleRooms().ToArray();

        public virtual IEnumerable<RoomCell> GetPossibleRooms()
        {
            RoomCell roomWithExecutor = RoomWithExecutor;
            IEnumerable<RoomCell> result =
                Map.Where(x => x.GetPassagesTo(roomWithExecutor).Length != 0 && x != roomWithExecutor);
            
            return result;
        }

        public virtual IGameAction.CanExecuteCheckResult CanExecute()
        {
            RoomCell[] selectedRooms = RoomSelection; 

            if (Executor.ActionCount.Value <= 0)
            {
                IGameAction.CanExecuteCheckResult result = new()
                {
                    Result = false,
                    Error = new InvalidOperationException($"Not enough action points to execute action"),
                };

                return result;
            }

            if (selectedRooms.Length != 1)
            {
                IGameAction.CanExecuteCheckResult result = new()
                {
                    Result = false,
                    Error = new InvalidOperationException($"To execute {nameof(MoveAction)}, room selection must contains only 1 room"),
                };

                return result;
            }

            RoomCell selectedRoom = selectedRooms.First();

            IEnumerable<RoomCell> possibleRooms = GetPossibleRooms();
            
            bool boolResult = possibleRooms.Contains(selectedRoom);
            
            return new()
            {
                Result = boolResult,
                Error = boolResult ? null : new InvalidOperationException($"No path to move in selected room"),
            };
        }

        public virtual void Execute()
        {
            IGameAction.CanExecuteCheckResult chekResult = CanExecute();
            if (chekResult == false)
            {
                throw chekResult.Error;
            }

            ForceExecute();
        }

        public virtual void ForceExecute()
        {
            RoomCell selectedRoom = RoomSelection.First();

            Executor.ActionCount.Value--;
            MoveNoiseBlocker[] blockers = selectedRoom.GetContentWith<MoveNoiseBlocker>().ToArray();
            
            selectedRoom.AddContent(Executor.CharacterPawn.RoomContent);

            if (selectedRoom.IsExplored.Value == false)
            {
                selectedRoom.Explore();
            }

            if (blockers.Length == 0)
            {
                _ = NoiseAfterMove(selectedRoom);
            }
        }

        private async UniTask NoiseAfterMove(RoomCell selectedRoom)
        {
            NoiseDice.Result result = await Map.NoiseInRoom(selectedRoom);
            Debug.Log($"Dice roll move result: {result}");
        }

        public void Initialzie(Map map)
        {
            Map = map;
        }

        public void Inititalize(PlayerTablet executer)
        {
            Executor = executer;
        }
    }
}