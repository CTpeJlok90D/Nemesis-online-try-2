using Core.DestinationCoordinats;
using Cysharp.Threading.Tasks;
using UI.Selection;

namespace Core
{
    public interface INeedCoordinates
    {
        public int RequiredCoordinatesAmount { get; }
        
        public Coordinate[] CoordinatesSource { get; }
        
        public Coordinate[] CoordinatesSelection { get; set; }
        
        internal async UniTask<Coordinate[]> GetSelectionLocal(CoordinatesSelection selection)
        {
            Coordinate[] selected = await selection.SelectFrom(CoordinatesSource, RequiredCoordinatesAmount);

            CoordinatesSelection = selected;
            return selected;
        }
    }
}