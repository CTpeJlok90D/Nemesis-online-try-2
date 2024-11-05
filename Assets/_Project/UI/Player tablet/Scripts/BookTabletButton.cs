using Core.Players;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.PlayerTablets
{
    public class BookTabletButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PlayerTabletContainer _playerTabletContainer;

        public void OnPointerClick(PointerEventData eventData)
        {
            _ = _playerTabletContainer.PlayerTablet.ToBookItFor(Player.Local);
            Debug.Log("Clicked");
        }
    }
}
