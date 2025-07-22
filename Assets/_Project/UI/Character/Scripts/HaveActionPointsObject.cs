using Core.PlayerTablets;
using TNRD;
using UnityEngine;

namespace UI.Characters
{
    public class HaveActionPointsObject : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IContainsPlayerTablet> _playerTablet;
        
        [SerializeField] private GameObject _target;

        public PlayerTablet PlayerTablet => _playerTablet.Value.PlayerTablet;

        private void OnEnable()
        {
            PlayerTablet.ActionCount.Changed += OnActionCountChange;
            if (didStart)
            {
                UpdateSize();
            }
        }

        private void Start()
        {
            UpdateSize();
        }

        private void OnDisable()
        {
            PlayerTablet.ActionCount.Changed -= OnActionCountChange;
        }

        private void OnActionCountChange(int previousValue, int newValue) => UpdateSize();
        private void UpdateSize()
        {
            _target.SetActive(PlayerTablet.ActionCount.Value > 0);
        }
    }
}
