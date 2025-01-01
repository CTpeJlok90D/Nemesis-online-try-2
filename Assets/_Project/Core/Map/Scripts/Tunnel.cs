using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-road-96.png")]
    [RequireComponent(typeof(SimpleNoiseContainer), typeof(NetworkObject))]
    public class Tunnel : NetworkBehaviour
    {
        [SerializeField] private RoomCell[] _linkedCells;

        [SerializeField] private DoorState _doorState;

        private NetVariable<DoorState> _doorStateNet;

        public DoorState DoorState => _doorStateNet.Value;

        public IReadOnlyCollection<RoomCell> RoomCells => _linkedCells;

        private void Awake()
        {
            _doorStateNet = new(_doorState);
        }

        public void Open()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can open doors");
            }

            if (DoorState is DoorState.Broken)
            {
                throw new InvalidOperationException("Door is broken");
            }

            _doorStateNet.Value = DoorState.Opened;
        }

        public void Close()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can close doors");
            }

            if (DoorState is DoorState.Broken)
            {
                throw new InvalidOperationException("Door is broken");
            }

            _doorStateNet.Value = DoorState.Closed;
        }

        public void Broke()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can broke doors");
            }

            if (DoorState is DoorState.Broken)
            {
                throw new InvalidOperationException("Door is already broken");
            }

            _doorStateNet.Value = DoorState.Broken;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            foreach (RoomCell roomCell in _linkedCells)
            {
                Gizmos.DrawLine(transform.position, roomCell.transform.position);
            }
        }
#endif
    }
}
