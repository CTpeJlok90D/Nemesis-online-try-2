using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Characters
{
    [Icon("Assets/_Project/Core/Characters/Editor/icons8-person-96.png")]
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject, INetworkSerializable, IEquatable<Character>, INetScriptableObjectArrayElement<Character>
    {
        [field: SerializeField] private string _id;
        [field: SerializeField] private NetScriptableObject<Character> _net = new();

        public NetScriptableObject<Character> Net => _net;

        public string Id => _id;

        public bool Equals(Character other)
        {
            return
                _id == other._id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            FixedString128Bytes characterID = new();
            if (serializer.IsWriter)
            {
                characterID = _id;
            }
            serializer.SerializeValue(ref characterID);
            if (serializer.IsReader)
            {
                _id = characterID.ToString();
            }

            _net.OnNetworkSerialize(serializer, this);

            if (string.IsNullOrEmpty(name))
            {
                name = $"{_id} (net loaded)";
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                _id = name;
            }
        }
#endif
    }   
}