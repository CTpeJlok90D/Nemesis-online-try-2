using System.Linq;
using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class LocalPlayerIsInfectedToken : MonoBehaviour
    {
        [SerializeField] private GameObject _isInfectedIndicator;

        private void OnEnable()
        {
            PlayerTablet.Local.TagAdded += OnAdd;
            PlayerTablet.Local.TagRemoved += OnRemove;
            UpdateActive();
        }

        private void OnDisable()
        {
            PlayerTablet.Local.TagAdded -= OnAdd;
            PlayerTablet.Local.TagRemoved -= OnRemove;
        }

        private void OnRemove(PlayerTablet sender, PlayerTag tag) => UpdateActive();
        private void OnAdd(PlayerTablet sender, PlayerTag tag) => UpdateActive();
        private void UpdateActive()
        {
            _isInfectedIndicator.SetActive(PlayerTablet.Local.Tags.Contains(PlayerTag.Larvae));
        }
    }
}