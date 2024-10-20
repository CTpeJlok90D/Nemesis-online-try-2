using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Common
{
    public class ExitButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Application.Quit();
        }
    }
}
