using AYellowpaper;
using Core.PlayerTablets;
using UnityEngine;

namespace UI.Characters
{
    public class HaveActionPointsObject : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IContainsPlayerTablet> _playerTablet;
        [SerializeField] private GameObject _target;

        public PlayerTablet PlayerTablet => _playerTablet.Value.PlayerTablet;

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
            PlayerTablet.ActionCount.Changed += OnActionCountChange;
        }

        private void OnActionCountChange(int previousValue, int newValue) => UpdateSize();
        private void UpdateSize()
        {
            _target.SetActive(TargetActive);
        }
    }
}
