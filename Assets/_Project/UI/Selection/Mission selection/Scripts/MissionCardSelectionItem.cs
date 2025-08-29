using Core;
using Core.Missions;
using Core.SelectionBase;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(PointerEvents))]
    public class MissionCardSelectionItem : MonoBehaviour
    {
        [SerializeField] private MissionContainer _container;
        [SerializeField] private GameObject _isSelectedObject;
        
        [Inject] private MissionSelection _missionSelection;

        public Mission Mission => _container.Mission;
        
        private PointerEvents _pointerEvents;

        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
        }

        private void OnEnable()
        {
            _missionSelection.Changed += OnSelectionChange;
            _pointerEvents.PointerClicked += OnPointerClick;
        }

        private void OnDisable()
        {
            _missionSelection.Changed -= OnSelectionChange;
            _pointerEvents.PointerClicked -= OnPointerClick;
        }

        private void OnPointerClick(PointerEvents sender, PointerEventData eventData)
        {
            _missionSelection.Add(Mission);
        }

        private void OnSelectionChange(ISelection sender)
        {
            _isSelectedObject.SetActive(_missionSelection.IsSelected(Mission));
        }
    }
}