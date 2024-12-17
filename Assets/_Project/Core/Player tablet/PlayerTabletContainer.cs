using Core.Characters;
using Core.Players;
using UnityEngine;

namespace Core.PlayerTablets
{
    public class PlayerTabletContainer : MonoBehaviour, IContainsPlayerTablet, IContainsCharacter, IContainsPlayer
    {
        public PlayerTablet PlayerTablet { get; private set; }

        public Character Character => PlayerTablet.Character.Value;

        public Player Player => PlayerTablet.Player;

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
