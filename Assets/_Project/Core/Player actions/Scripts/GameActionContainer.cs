using System;
using AYellowpaper;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.PlayerActions
{
    [CreateAssetMenu(menuName = "Game/Actions/Action container")]
    public class GameActionContainer : ScriptableObject, INetworkSerializable, INetScriptableObjectArrayElement<GameActionContainer>, IEquatable<GameActionContainer>
    {
        [field: SerializeField] public InterfaceReference<IGameAction> GameAction { get; private set; }
        [field: SerializeField] public NetScriptableObject<GameActionContainer> Net { get; private set; }

        public bool Equals(GameActionContainer other)
        {
            return Net.RuntimeLoadKey.Equals(other.Net.RuntimeLoadKey);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Net.OnNetworkSerialize(serializer, this);
        }
    }
}
