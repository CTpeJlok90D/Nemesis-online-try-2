using Core.Entities;
using UnityEngine;
using Unity.Netcode;
using Zenject;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    public class RoomContent : NetEntity<RoomContent>
    {
        [Inject] private Map _map;
        
        public RoomCell Owner { get; internal set; }

        public delegate void DespawnedHandler(RoomContent sender);
        public static event DespawnedHandler Despawned;

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Despawned?.Invoke(this);
        }
    }
}
