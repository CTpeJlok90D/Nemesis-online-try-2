using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.DestinationCoordinats
{
    [CreateAssetMenu(menuName = "Game/Maps/Control point/Coordinate")]
    [Icon("Assets/_Project/Core/Destenation coordinats/Editor/icons8-navigate-96.png")]
    public class Coordinate : ScriptableObject, INetworkSerializable, IEquatable<Coordinate>
    {
        [field: SerializeField] public NetScriptableObject<Coordinate> _net = new();

        public bool Equals(Coordinate other)
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
