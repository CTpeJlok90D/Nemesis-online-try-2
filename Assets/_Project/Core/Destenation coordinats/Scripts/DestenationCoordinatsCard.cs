using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.DestinationCoordinats
{
    [CreateAssetMenu(menuName = "Game/Maps/Control point/Destination coordinats card")]
    [Icon("Assets/_Project/Core/Destenation coordinats/Editor/icons8-map-96.png")]
    public class DestinationCoordinatsCard : ScriptableObject, INetworkSerializable, IEquatable<DestinationCoordinatsCard>, INetScriptableObjectArrayElement<DestinationCoordinatsCard>
    {
        [field: SerializeField] private SerializedDictionary<Coordinate, Destination> _coordinateForDestenation;

        [field: SerializeField] public NetScriptableObject<DestinationCoordinatsCard> _net = new();

        public IReadOnlyDictionary<Coordinate, Destination> CoordinatesForDestinations => _coordinateForDestenation;

        public NetScriptableObject<DestinationCoordinatsCard> Net => _net;

        public bool Equals(DestinationCoordinatsCard other)
        {
            return 
                _net.SelfAssetReference.RuntimeKey == other._net.SelfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.Preloaded += OnLoad;
            _net.OnNetworkSerialize(serializer, this);
        }

        private void OnLoad(DestinationCoordinatsCard result)
        {
            _net.Preloaded -= OnLoad;
            _coordinateForDestenation = new(result._coordinateForDestenation);
        }
    }
}
