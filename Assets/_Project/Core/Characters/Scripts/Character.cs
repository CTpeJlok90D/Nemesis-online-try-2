using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core.Characters
{
    [Icon("Assets/_Project/Core/Characters/Editor/icons8-person-96.png")]
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject, INetworkSerializable, IEquatable<Character>
    {
        [SerializeField] public string _id;

        public string Id => _id;

        public bool Equals(Character other)
        {
            return
                _id == other._id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            FixedString64Bytes id = new();
            if (serializer.IsWriter)
            {
                id = new(_id);
            }
            serializer.SerializeValue(ref id);
            if (serializer.IsReader)
            {
                _id = id.ToString();
            }

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