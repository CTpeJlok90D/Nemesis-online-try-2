using Core.Missions;
using Core.PlayerTablets;
using TNRD;
using UnityEngine;

namespace UI
{
    public class MissionIsCompletedObject : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IContainsMission> _missionContainer;
        [SerializeField] private SerializableInterface<IContainsPlayerTablet> _playerTabletContainer;
        [SerializeField] private GameObject _target;
        [SerializeField] private TMode _mode;
        [SerializeField] private bool _inverse;

        private Mission Mission => _missionContainer.Value.Mission;
        private PlayerTablet PlayerTabletContainer => _playerTabletContainer.Value.PlayerTablet;
        
        private void OnEnable()
        {
            UpdateObject();
        }

        private void UpdateObject()
        {
            bool result;
            switch (_mode)
            {
                case TMode.Survive:
                    result = Mission.IsSurvivedFor(PlayerTabletContainer);
                    if (_inverse)
                    {
                        result = !result;
                    }
                    _target.SetActive(result);
                    break;
                case TMode.Target:
                    result = Mission.IsCompletedFor(PlayerTabletContainer);
                    if (_inverse)
                    {
                        result = !result;
                    }
                    _target.SetActive(result);
                    break;
            }
        }
        
        private enum TMode
        {
            Target,
            Survive
        }
    }
}