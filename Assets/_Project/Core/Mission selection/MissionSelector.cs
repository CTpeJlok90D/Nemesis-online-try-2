using System.Linq;
using Core.Aliens;
using Core.Entities;
using Core.Missions;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace Core
{
    public enum MissionChooseState
    {
        NotSelected,
        InProgress,
        Selected,
    }
    
    public class MissionSelector : NetEntity<MissionSelector>
    {
        public const int MissionsToSelect = 1;
        private NetVariable<MissionChooseState> _currentMissionInChoosing;

        [Inject] private MissionSelection _selection;

        public IReadOnlyReactiveField<MissionChooseState> CurrentMissionChooseState => _currentMissionInChoosing;
        
        private void Awake()
        {
            _currentMissionInChoosing = new(MissionChooseState.NotSelected);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Enemy.Spawned += OnEnemySpawn;
            _currentMissionInChoosing.Changed += OnCurrentMissionChooseStateChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Enemy.Spawned -= OnEnemySpawn;
            _currentMissionInChoosing.Changed -= OnCurrentMissionChooseStateChange;
        }

        private void OnCurrentMissionChooseStateChange(MissionChooseState oldValue, MissionChooseState newValue)
        {
            if (NetworkManager.IsServer && newValue is MissionChooseState.InProgress)
            {
                _ = AwaitPlayersSelecting();
            }
        }

        private async UniTask AwaitPlayersSelecting()
        {
            ForceToSelectMissions_RPC();
            while (PlayerTablet.Instances.Any(x => x.Missions.Count > MissionsToSelect))
            {
                await UniTask.NextFrame();
            }
            _currentMissionInChoosing.Value = MissionChooseState.Selected;
        }

        private void OnEnemySpawn(Enemy spawned)
        {
            Enemy.Spawned -= OnEnemySpawn;
            if (NetworkManager.IsServer)
            {
                _currentMissionInChoosing.Value = MissionChooseState.InProgress;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ForceToSelectMissions_RPC()
        {
            _ = SelectMission();
        }

        private async UniTask SelectMission()
        {
            Mission[] missions = await _selection.SelectFrom(PlayerTablet.Local.Missions, MissionsToSelect);
            Mission selectedMission = missions.First();
            ChooseMission_RPC(selectedMission, PlayerTablet.Local.NetworkObject);
        }

        [Rpc(SendTo.Server)]
        private void ChooseMission_RPC(Mission mission, NetworkObjectReference playerTablet)
        {
            PlayerTablet sender;

            if (playerTablet.TryGet(out NetworkObject senderNetObject))
            {
                sender = senderNetObject.GetComponent<PlayerTablet>();
            }
            else
            {
                Debug.LogError($"Cant convert NetworkObjectReference to playerTablet", this);
                return;
            }

            if (sender.Missions.Contains(mission))
            {
                sender.Missions.Clear();
                sender.Missions.Add(mission);
            }
            else
            {
                Debug.LogError($"Player tablet {sender} do not contains {mission} mission. Choosing first", sender);
                
                Mission firstMission = sender.Missions.First();
                sender.Missions.Clear();
                sender.Missions.Add(firstMission);
            }
        }
    }
}