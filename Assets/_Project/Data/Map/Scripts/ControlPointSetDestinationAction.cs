using System.Linq;
using Core.DestinationCoordinats;
using Core.Maps;
using Core.PlayerActions;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Game/Maps/Actions/Control point set destination action")]
    public class ControlPointSetDestinationAction : RoomAction, INeedCoordinates
    {
        [SerializeField] private Coordinate[] _availableCoordinates;

        public int RequiredCoordinatesAmount => 1;
        public Coordinate[] CoordinatesSource => _availableCoordinates;
        public Coordinate[] CoordinatesSelection { get; set; }
        
        public override IGameAction.CanExecuteCheckResult CanExecute()
        {
            return new()
            {
                Result = true
            };
        }

        public override void ForceExecute()
        {
            Executor.ActionCount.Value--;
            
            Coordinate selectedCoordinate = CoordinatesSelection.First();
            Ship.Instance.Coordinate.Value = selectedCoordinate;
        }
    }
}