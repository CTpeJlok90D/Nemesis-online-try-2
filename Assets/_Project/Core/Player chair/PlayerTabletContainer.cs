using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.PlayerTablets
{
    public class PlayerTabletContainer : MonoBehaviour
    {
        public PlayerTablet PlayerTablet { get; private set; }

        public PlayerTabletContainer Instantiate(PlayerTablet tablet, Transform parent = null)
        {
            gameObject.SetActive(false);
            PlayerTabletContainer result = Instantiate(this, parent);
            gameObject.SetActive(true);

            result.PlayerTablet = tablet;
            result.gameObject.SetActive(true);

            return result;
        }
    }
}
