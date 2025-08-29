using System.Collections.Generic;
using Core;
using Core.Missions;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace UI.Selection
{
    public class MissionCardsSelectionSpawner : MonoBehaviour
    {
        [SerializeField] private MissionContainer _missionCard_PREFAB;
        [SerializeField] private Transform _cardsParent;
        [Inject] private MissionSelection _selection;

        private List<MissionContainer> _missionContainers = new();
        
        private void OnEnable()
        {
            UpdateInstances();
            _selection.Started += OnSelectionStart;
            _selection.Confirmed += OnSelectionConfirm;
            _selection.Canceled += OnSelectionCancel;
        }

        private void OnDisable()
        {
            _selection.Started -= OnSelectionStart;
            _selection.Confirmed -= OnSelectionConfirm;
            _selection.Canceled -= OnSelectionCancel;
        }

        private void OnSelectionCancel(ISelection sender) => UpdateInstances();
        private void OnSelectionConfirm(ISelection sender) => UpdateInstances();
        private void OnSelectionStart(ISelection sender) => UpdateInstances();
        private void UpdateInstances()
        {
            DestroyCards();
            InstantiateCards();
        }

        private void InstantiateCards()
        {
            foreach (Mission mission in _selection.SelectionSource)
            {
                MissionContainer missionContainer = _missionCard_PREFAB.Instantiate(mission, _cardsParent);
                _missionContainers.Add(missionContainer);
            }
        }

        private void DestroyCards()
        {
            foreach (MissionContainer container in _missionContainers)
            {
                Destroy(container.gameObject);
            }
            _missionContainers.Clear();
        }
    }
}