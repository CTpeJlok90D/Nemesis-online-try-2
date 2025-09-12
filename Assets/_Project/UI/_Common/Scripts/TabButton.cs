using UI.Common;
using UI.CommonScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Tab _tab;
        [SerializeField] private PointerEvents _pointerEvents;

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnPointerClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnPointerClick;
        }

        private void OnPointerClick(PointerEvents sender, PointerEventData eventData)
        {
            if (_tab.gameObject.activeSelf)
            {
                _tab.gameObject.SetActive(false);
                return;
            }
            _tab.Enable();
        }
    }
}