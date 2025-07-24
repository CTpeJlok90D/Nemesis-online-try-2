using System;
using Core.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Custom;
using Zenject;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    public class RoomContent : NetEntity<RoomContent>
    {
        [Inject] private Ship _ship;
        
        private NetBehaviourReference<RoomCell> _ownerNet { get; set; }

        public RoomCell Owner
        {
            get
            {
                return _ownerNet.Reference;
            }
            internal set
            {
                _ownerNet.Reference = value;
            }
        }

        public event NetBehaviourReference<RoomCell>.ReferenceChangedListener OwnerChanged
        {
            add => _ownerNet.ReferenceChanged += value;
            remove => _ownerNet.ReferenceChanged -= value;
        }

        public delegate void DespawnedHandler(RoomContent sender);
        public new static event DespawnedHandler Despawned;

        private void Awake()
        {
            _ownerNet = new();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Despawned?.Invoke(this);
        }
    }
}
