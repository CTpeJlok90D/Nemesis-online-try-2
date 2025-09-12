using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ZoomableObject : MonoBehaviour
    {
        [SerializeField] private PointerEvents _pointerEvents;
        [SerializeField] private Vector3 _scale = new(1.6f, 1.6f, 1.6f);

        private GameObject _zoomedCopy;
        
        private void OnEnable()
        {
            _pointerEvents.PointerEnter += OnPointerEnter;
            _pointerEvents.PointerExit += OnPointerExit;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerEnter -= OnPointerEnter;
            _pointerEvents.PointerExit -= OnPointerExit;
        }
        
        private void OnPointerEnter(PointerEvents sender, PointerEventData eventData)
        {
            ZoomableObject copy = Instantiate(this, ZoomCanvas.Instance.transform);
            _zoomedCopy = copy.gameObject;
            
            RectTransform rectTransform = (RectTransform)_zoomedCopy.transform;
            
            rectTransform.localScale = _scale;
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)ZoomCanvas.Instance.transform,
                Input.mousePosition,
                ZoomCanvas.Instance.Canvas.worldCamera,
                out mousePosition
            );
            rectTransform.anchoredPosition = mousePosition;
            
            RectBorders borders = _zoomedCopy.AddComponent<RectBorders>();
            borders.Init((RectTransform)_zoomedCopy.transform, (RectTransform)ZoomCanvas.Instance.transform);
            Destroy(copy);
        }
        
        private void OnPointerExit(PointerEvents sender, PointerEventData eventData)
        {
            if (_zoomedCopy != null)
            {
                Destroy(_zoomedCopy);
            }
        }
    }
}