using System.Linq;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.Starter
{
    public class TabletsCleaner : MonoBehaviour
    {
        private void OnEnable()
        {
            Activator.Singleton.GameActivated += OnGameActive;
        }

        private void OnDisable()
        {
            Activator.Singleton.GameActivated -= OnGameActive;
        }

        private void OnGameActive()
        {
            foreach (PlayerTablet tablet in PlayerTablet.Instances.Where(x => x.IsEmpty))
            {
                tablet.NetworkObject.Despawn();
            }
        }
    }
}
