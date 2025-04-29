using System;
using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using UnityEngine;

namespace Core.PlayerActions.Base
{
    [CreateAssetMenu(menuName = Constants.ACTIONS_CREATE_PARH + "Careful move action")]
    public class CarefulMovement : MoveAction, INeedNoiseContainers
    {
        public override int RequaredPaymentCount => 2;
        public int RequiredNoiseContainerCount => 1;
        private INoiseContainer _selectedNoiseContainer;
        public INoiseContainer[] SelectedNoiseContainers
        {
            get
            {
                if (_selectedNoiseContainer == null)
                {
                    return Array.Empty<INoiseContainer>();
                }
                return new [] { _selectedNoiseContainer };
            }
            set
            {
                if (value.Length == 0)
                {
                    _selectedNoiseContainer = null;
                    return;
                }
                _selectedNoiseContainer = value[0];

                if (value.Length > 1)
                {
                    Debug.LogError($"{this} requires only 1 noise сontainer. Other containers will be ignored",this);
                }
            }
        }

        public INoiseContainer[] NoiseContainerSelectionSource
        {
            get
            {
                RoomCell selectedRoomCell = RoomSelection.First();
                return selectedRoomCell.NoiseContainers.Where(x => x.IsNoised.Value == false).ToArray();
            }
        }

        public override IEnumerable<RoomCell> GetPossibleRooms()
        {
             IEnumerable<RoomCell> rooms = base.GetPossibleRooms();
             return rooms.Where(x => x.NoiseContainers.Any(y => y.IsNoised.Value == false));
        }

        public override void ForceExecute()
        {
            RoomCell selectedRoom = RoomSelection.First();

            Executor.ActionCount.Value--;
            selectedRoom.AddContent(Executor.CharacterPawn.RoomContent);
            
            Map.CarefulNoiseInTunnel(_selectedNoiseContainer);
        }
    }
}
