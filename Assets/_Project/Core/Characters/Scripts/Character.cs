using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Characters
{
    [Icon("Assets/_Project/Core/Characters/Editor/icons8-person-96.png")]
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject, INetworkSerializable, IEquatable<Character>
    {
        [SerializeField] public FixedString64Bytes _id;

        public string Id => _id.ToString();

        public bool Equals(Character other)
        {
            return
                _id.Equals(other._id);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _id);
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