using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Core.Maps;
using UnityEngine;

namespace View.Tunnels
{
    [DefaultExecutionOrder(1)]
    public class DoorView : MonoBehaviour
    {
        [SerializeField] private Tunnel _tunnel;
        [SerializeField] private SerializedDictionary<DoorState, GameObject> _doorStateViews;
        
        private void OnEnable()
        {
            _tunnel.DoorStateChanged += OnDoorStateChange;
            UpdateDoorState();
        }
        
        private void Start()
        {
            UpdateDoorState();
        }

        private void OnDisable()
        {
            _tunnel.DoorStateChanged -= OnDoorStateChange;
        }

        private void OnDoorStateChange(DoorState oldvalue, DoorState newvalue)
        {
            UpdateDoorState();
        }

        private void UpdateDoorState()
        {
            foreach (KeyValuePair<DoorState, GameObject> doors in _doorStateViews)
            {
                doors.Value.SetActive(false);
            }

            if (_doorStateViews.TryGetValue(_tunnel.DoorState, out var doorState))
            {
                doorState.SetActive(true);
            }
        }
    }
}
