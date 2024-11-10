using System.Threading.Tasks;
using Core.Players;
using Core.PlayerTablets;
using UI.Loading;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.PlayerTablets
{
    public class BookTabletButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PlayerTabletContainer _playerTabletContainer;
        [Inject] private LoadScreen _loadScreen;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_playerTabletContainer.PlayerTablet.CanBookIt(Player.Local))
            {
                Task bookTask = _playerTabletContainer.PlayerTablet.ToBookItFor(Player.Local);
            }
        }
    }
}
