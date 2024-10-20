using System.Threading.Tasks;
using Core.Starter;
using UI.Loading;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Common
{
    public class StartGameButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private Activator _activator;
        [Inject] private LoadScreen _loadScreen;
        public void OnPointerClick(PointerEventData eventData)
        {
            Task loadTask = _activator.StartGame();
            _ = _loadScreen.StartLoading(loadTask);
        }
    }
}
