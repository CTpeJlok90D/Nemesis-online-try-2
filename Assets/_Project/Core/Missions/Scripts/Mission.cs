using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Missions
{
    [Icon("Assets/_Project/Core/Missions/Editor/icons8-mission-96.png")]
    [CreateAssetMenu(menuName = "Game/Players/Mission")]
    public class Mission : ScriptableObject, INetScriptableObjectArrayElement<Mission>, INetworkSerializable, IEquatable<Mission>
    {
        [field: SerializeField] public MissionType Type { get; private set; }

        [field: SerializeField] private NetScriptableObject<Mission> _net = new();

        [field: SerializeField] public int MinPlayerCount { get; private set; }

        public NetScriptableObject<Mission> Net => _net;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.OnNetworkSerialize(serializer, this);
            _net.Loaded += OnLoad;
        }

        private void OnLoad(Mission result)
        {
            _net.Loaded -= OnLoad;
            MinPlayerCount = result.MinPlayerCount;
            if (string.IsNullOrEmpty(name))
            {
                name = $"{result.name} (net loaded)";
            }
        }

        public bool Equals(Mission other)
        {
            return other._net.SelfAssetReference.RuntimeKey.Equals(_net.SelfAssetReference.RuntimeKey);
        }
    }

    public enum MissionType
    {
        Personal, Сorporate
    }
}
