using System.Linq;
using Core.PlayerTablets;
using UnityEngine;

namespace UI.GameActions
{
    [DefaultExecutionOrder(1)]
    public class CanDropItemObject : MonoBehaviour
    {
        [SerializeField] private GameObject _target;

        private PlayerTablet LocalTablet => PlayerTablet.Local;

        private void Update()
        {
            UpdateObject();
        }

        private void UpdateObject()
        {
            if (LocalTablet == null)
            {
                _target.SetActive(false);
                return;
            }
            
            _target.SetActive(LocalTablet.BigItemsInventory != null && LocalTablet.BigItemsInventory.Any() ||
                              LocalTablet.SmallItemsInventory != null && LocalTablet.SmallItemsInventory.Any());
        }
    }
}