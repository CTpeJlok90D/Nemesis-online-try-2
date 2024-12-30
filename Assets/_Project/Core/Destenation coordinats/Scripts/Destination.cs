using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.DestinationCoordinats
{
    [CreateAssetMenu(menuName = "Game/Maps/Control point/Destination")]
    [Icon("Assets/_Project/Core/Destenation coordinats/Editor/icons8-planet-96.png")]
    public class Destination : ScriptableObject, INetworkSerializable, IEquatable<Destination>, INetScriptableObjectArrayElement<Destination>
    {
        [field: SerializeField] public NetScriptableObject<Destination> _net;

        public NetScriptableObject<Destination> Net => _net;

        public bool Equals(Destination other)
        {
            return 
                _net.SelfAssetReference.RuntimeKey == other._net.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.OnNetworkSerialize(serializer, this);
        }
    }
}
