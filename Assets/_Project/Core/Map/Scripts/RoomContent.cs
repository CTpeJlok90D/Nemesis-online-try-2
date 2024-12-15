using UnityEngine;
using Unity.Netcode;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    public class RoomContent : NetworkBehaviour
    {
        public RoomCell Owner { get; internal set; }
    }
}
