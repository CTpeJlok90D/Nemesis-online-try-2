using System.Collections.Generic;
using System.Linq;
using Core.Missions;
using Core.PlayerTablets;
using Unity.Netcode.Custom;
using UnityEngine;

namespace UI
{
    public class MissionsCardsSpawner : MonoBehaviour
    {
        [SerializeField] private MissionContainer _missionContainerPrefab;
        [SerializeField] private Transform _missionsParent;

        private readonly List<MissionContainer> _instances = new();
        
        private void OnEnable()
        {
            PlayerTablet.Local.Missions.ListChanged += OnMissionsListChange;
            PlayerTablet.LocalChanged += OnLocalTabletChange;
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            {
                PlayerTablet.Local.Missions.ListChanged -= OnMissionsListChange;
            }
            PlayerTablet.LocalChanged -= OnLocalTabletChange;
        }

        private void OnLocalTabletChange(PlayerTablet old, PlayerTablet newValue)
        {
            old.Missions.ListChanged -= OnMissionsListChange;
            newValue.Missions.ListChanged += OnMissionsListChange;
        }

        private void OnMissionsListChange(NetScriptableObjectList4096<Mission> sender)
        {
            UpdateMissions();
        }

        private void Start()
        {
            UpdateMissions();
        }

        private void UpdateMissions()
        {
            DestroyInstances();
            SpawnInstances();
        }

        private void SpawnInstances()
        {
            foreach (Mission mission in PlayerTablet.Local.Missions.ToArray())
            {
                MissionContainer missionContainer = _missionContainerPrefab.Instantiate(mission, _missionsParent);
                _instances.Add(missionContainer);
            }
        }

        private void DestroyInstances()
        {
            foreach (MissionContainer mission in _instances)
            {
                Destroy(mission.gameObject);
            }

            _instances.Clear();
        }
    }
}