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
        [field: SerializeField] public NetScriptableObject<GameActionContainer> Net { get; private set; } = new();

        public bool Equals(GameActionContainer other)
        {
            return Net.RuntimeLoadKey.Equals(other.Net.RuntimeLoadKey);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Net.Loaded += OnNetLoad;
            Net.OnNetworkSerialize(serializer, this);
        }

        private void OnNetLoad(GameActionContainer result)
        {
            Net.Loaded -= OnNetLoad;
            GameAction = result.GameAction;
        }
    }
}
