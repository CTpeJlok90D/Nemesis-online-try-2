using Core.AliensTablets;
using UnityEngine;

namespace Data.AliensTablet
{
    [CreateAssetMenu(menuName = "Game/Aliens/Test alient weakness")]
    public class TestWeakness : AlienWeakness
    {
        private bool _isActive;
        public override bool IsActive 
        {
            get 
            {
                return _isActive;
            }
            set 
            {
                _isActive = value;
                Debug.Log($"Test weakness active was changed: {_isActive}");
            }
        }
    }
}
