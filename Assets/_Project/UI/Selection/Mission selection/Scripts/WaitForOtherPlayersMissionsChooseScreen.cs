using System;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Missions;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UI.Loading;
using Unity.Netcode.Custom;
using UnityEngine;
using Zenject;

namespace UI
{
    public class WaitForOtherPlayersMissionsChooseScreen : MonoBehaviour
    {
        [SerializeField] private MissionSelector _missionSelector;

        [Inject] private LoadScreen _loadScreen;
        
        private void OnEnable()
        {
            PlayerTablet.Local.Missions.ListChanged += OnMissionsChange;
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            {
                PlayerTablet.Local.Missions.ListChanged -= OnMissionsChange;
            }
        }

        private void OnMissionsChange(NetScriptableObjectList4096<Mission> sender)
        {
            if (PlayerTablet.Local.Missions.Count == MissionSelector.MissionsToSelect 
                && _missionSelector.CurrentMissionChooseState.Value is MissionChooseState.InProgress)
            {
                _ = _loadScreen.Show(AwaitOtherPlayers());
            }
        }

        private async Task AwaitOtherPlayers()
        {
            try
            {
                while (_missionSelector.CurrentMissionChooseState.Value is MissionChooseState.InProgress)
                {
                    await UniTask.NextFrame();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}