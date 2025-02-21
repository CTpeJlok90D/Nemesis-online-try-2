using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Common
{
    public class PointerEvents : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        public delegate void PointerEventListener(PointerEvents pointerEvents, PointerEventData eventData);

        public event PointerEventListener PointerClicked;
        public event PointerEventListener PointerUp;
        public event PointerEventListener PointerDown;

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClicked?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(this, eventData);
        }
    }
}
