using System;
using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.Maps.IntellegenceTokens;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.PlayerActions
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Actions + "Move action")]
    public class MoveAction : ScriptableObject, IGameAction, INeedPayment, INeedRooms, INeedMap
    {
        [SerializeField] private IntelegenceToken _noNoiseToken;
        [SerializeField] private IntelegenceToken _dangerousToken;
        public Ship Ship { get; private set; }
        public PlayerTablet Executor { get; private set; }
        public RoomCell[] RoomSelection { get; set; }
        public virtual int RequaredPaymentCount => 1;
        public int RequredRoomsCount => 1;
        public RoomCell RoomWithExecutor => Ship.First(x => x.RoomContents.Contains(Executor.CharacterPawn.RoomContent));
        public RoomCell[] RoomSelectionSource => GetPossibleRooms().ToArray();

        public virtual IEnumerable<RoomCell> GetPossibleRooms()
        {
            RoomCell roomWithExecutor = RoomWithExecutor;
            IEnumerable<RoomCell> result =
                Ship.Where(x => x.GetPassagesTo(roomWithExecutor).Length != 0 && x != roomWithExecutor);
            
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

        public virtual void ForceExecute()
        {
            RoomCell selectedRoom = RoomSelection.First();
            RoomCell oldRoom = Executor.CharacterPawn.RoomContent.Owner;

            Executor.ActionCount.Value--;
            List<IMoveNoiseBlocker> blockers = selectedRoom.GetContentWith<IMoveNoiseBlocker>().ToList();
            
            selectedRoom.AddContent(Executor.CharacterPawn.RoomContent);

            if (selectedRoom.IsExplored.Value == false)
            {
                selectedRoom.Explore();
                if (selectedRoom.IntellegenceTokenNet.Value.Action is IMoveNoiseBlocker)
                {
                    blockers.Add(selectedRoom.IntellegenceTokenNet.Value.Action as IMoveNoiseBlocker);
                }

                selectedRoom.IntellegenceTokenNet.Value.Action?.Execute(selectedRoom, oldRoom, Executor);
            }

            
            if (blockers.Count == 0)
            {
                _ = NoiseAfterMove(selectedRoom);
            }
        }

        private async UniTask NoiseAfterMove(RoomCell selectedRoom)
        {
            NoiseDice.Result result = await Ship.NoiseInRoom(selectedRoom);
            Debug.Log($"Dice roll move result: {result}");
        }

        public void Initialzie(Ship ship)
        {
            Ship = ship;
        }

        public void Initialize(PlayerTablet executer)
        {
            Executor = executer;
        }
    }
}