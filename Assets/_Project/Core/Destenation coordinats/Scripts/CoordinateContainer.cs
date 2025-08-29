using Core.DestinationCoordinats;
using UnityEngine;

namespace Core
{
    public class CoordinateContainer : MonoBehaviour
    {
        private ReactiveField<Coordinate> _coordinate = new();

        public ReactiveField<Coordinate> Coordinate => _coordinate;
        
        public CoordinateContainer Instantiate(Coordinate coordinate = null, Transform parent = null)
        {
            gameObject.SetActive(false);
            CoordinateContainer result = Instantiate(this, parent);
            gameObject.SetActive(true);

            result._coordinate = new()
            {
                Value = coordinate
            };
            result.gameObject.SetActive(true);
            
            return result;
        }
    }
}