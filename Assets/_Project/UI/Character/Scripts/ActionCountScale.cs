using Core.PlayerTablets;
using DG.Tweening;
using TNRD;
using UnityEngine;

namespace UI.Characters
{
    public class ActionCountScale : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IContainsPlayerTablet> _playerTablet;
        [SerializeField] private Vector2 _noActionsSize = new(75,75);
        [SerializeField] private Vector2 _haveActionsSize = new(100,100);
        [SerializeField] private RectTransform _target;

        public PlayerTablet PlayerTablet => _playerTablet.Value.PlayerTablet;

        private Vector2 TargetSize => PlayerTablet.ActionCount.Value > 0 ? _haveActionsSize : _noActionsSize;

        private void OnEnable()
        {
            if (didStart)
            {
                PlayerTablet.ActionCount.Changed += OnActionCountChange;
                _target.sizeDelta = TargetSize;
            }
        }

        private void Start()
        {
            PlayerTablet.ActionCount.Changed += OnActionCountChange;
            _target.sizeDelta = TargetSize;
        }

        private void OnDisable()
        {
            PlayerTablet.ActionCount.Changed -= OnActionCountChange;
        }

        private void OnActionCountChange(int previousValue, int newValue) => UpdateSize();
        private void UpdateSize()
        {
            _target.DOSizeDelta(TargetSize, 0.5f);
        }
    }
}
