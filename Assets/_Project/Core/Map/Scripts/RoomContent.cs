using Core.Entity;
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
        protected override RoomContent Instance => this;
    }
}
