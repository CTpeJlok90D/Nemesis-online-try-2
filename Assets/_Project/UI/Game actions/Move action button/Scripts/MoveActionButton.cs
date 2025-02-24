using UnityEngine;
using UnityEngine.EventSystems;
using Core.PlayerActions;
using UI.Common;

namespace UI.SelectionBase
{
    [RequireComponent(typeof(PointerEvents))]
    public class MoveActionButton : MonoBehaviour
    {
        [SerializeField] private GameActionContainer _moveAction;
        private PointerEvents _pointerEvents;

        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnPointerClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnPointerClick;
        }

        private void OnPointerClick(PointerEvents pointerEvents, PointerEventData eventData)
        {
            ExecuteMoveAction();
        }

        private void ExecuteMoveAction()
        {
            PlayerActionExecutor.Instance.Execute(_moveAction);
        }
    }
}
