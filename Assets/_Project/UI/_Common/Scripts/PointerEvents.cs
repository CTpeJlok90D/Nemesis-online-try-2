using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Common
{
    public class PointerEvents : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void PointerEventListener(PointerEvents sender, PointerEventData eventData);

        public event PointerEventListener PointerClicked;
        public event PointerEventListener PointerUp;
        public event PointerEventListener PointerDown;
        public event PointerEventListener PointerEnter;
        public event PointerEventListener PointerExit;

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

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke(this, eventData);
        }
    }
}
