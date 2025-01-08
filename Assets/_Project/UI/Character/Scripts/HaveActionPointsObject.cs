using Core.PlayerTablets;
using TNRD;
using UnityEngine;

namespace UI.Characters
{
    public class HaveActionPointsObject : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IContainsPlayerTablet> _playerTablet;
        
        [SerializeField] private GameObject _target;

        public PlayerTablet PlayerTablet 
        {
            get
            {
                return _playerTablet.Value.PlayerTablet;
            }
        }

        private bool TargetActive => PlayerTablet.ActionCount.Value > 0;

        private void OnEnable()
        {
            PlayerTablet.ActionCount.Changed += OnActionCountChange;
            if (didStart)
            {
                _target.SetActive(TargetActive);
            }
        }

        private void Start()
        {
            _target.SetActive(TargetActive);
        }

        private void OnDisable()
        {
            PlayerTablet.ActionCount.Changed -= OnActionCountChange;
        }

        private void OnActionCountChange(int previousValue, int newValue) => UpdateSize();
        private void UpdateSize()
        {
            _target.SetActive(TargetActive);
        }
    }
}
