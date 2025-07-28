using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Scenarios
{
    public class ToggleGameObjectChapter : NetworkBehaviour, IChapter
    {
        [SerializeField] private GameObject[] Targets;
        [SerializeField] private bool _defaultValue = true;

        public event IChapter.EndedListener Ended;

        private NetVariable<bool> _isEnabled;

        private void Awake()
        {
            _isEnabled = new(_defaultValue);
        }

        private void Start()
        {
            Targets.ToList().ForEach(x => x.gameObject.SetActive(_isEnabled.Value));
        }

        public void Begin()
        {
            if (NetworkManager.IsServer)
            {
                _isEnabled.Value = !_isEnabled.Value;
                ToggleObject_RPC(_isEnabled.Value);
            }
            Ended?.Invoke(this);
        }

        [Rpc(SendTo.Everyone)]
        private void ToggleObject_RPC(bool value)
        {
            Targets.ToList().ForEach(x => x.gameObject.SetActive(value));
        }
    }
}
