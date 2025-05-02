using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.Characters.Tokens
{
    [Icon("Assets/_Project/Core/Character tokens/Editor/token.png")]
    [CreateAssetMenu(menuName = "Game/Characters/Token")]
    public class CharacterToken : ScriptableObject, INetworkSerializable, IEquatable<CharacterToken>, INetScriptableObjectArrayElement<CharacterToken>
    {
        [SerializeField] private NetScriptableObject<CharacterToken> _net = new();
        public NetScriptableObject<CharacterToken> Net => _net;

        public string ID => name;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.OnNetworkSerialize(serializer, this);
        }

        public bool Equals(CharacterToken other)
        {
            return other != null && other.Net.RuntimeLoadKey.Equals(Net.RuntimeLoadKey);
        }
    }
}
