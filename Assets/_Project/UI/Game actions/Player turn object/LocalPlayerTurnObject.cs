using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class LocalPlayerTurnObject : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        private void OnEnable()
        {
            PlayerTablet.Local.ActionCount.Changed += OnActionCountChange;
            UpdateObject();
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            {
                PlayerTablet.Local.ActionCount.Changed -= OnActionCountChange;
            }
        }

        private void OnActionCountChange(int oldValue, int newValue) => UpdateObject();
        private void UpdateObject()
        {
            _target.SetActive(PlayerTablet.Local.ActionCount.Value > 0);
        }
    }
}