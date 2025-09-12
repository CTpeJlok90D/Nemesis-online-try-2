using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Maps.IntellegenceTokens
{
    [Icon("Assets/_Project/Core/Map/Editor/intellegence-token.png")]
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token")]
    public class IntelegenceToken : ScriptableObject, INetworkSerializable, IEquatable<IntelegenceToken>, INetScriptableObjectArrayElement<IntelegenceToken>
    {
        [field: SerializeField] private string _loadKey;
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public int LootCount { get; private set; }
        [field: SerializeField] public IntelegenceTokenAction Action { get; private set; }
        [field: SerializeField] private NetScriptableObject<IntelegenceToken> _net = new();

        public NetScriptableObject<IntelegenceToken> Net => _net;
        
        public bool Equals(IntelegenceToken other)
        {
            return other._loadKey == _loadKey && other.Id == Id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _net.Loaded += OnNetLoad;
            _net.OnNetworkSerialize(serializer, this);
        }

        private void OnNetLoad(IntelegenceToken result)
        {
            _net.Loaded -= OnNetLoad;
            
            _loadKey = result._loadKey;
            Action = result.Action;
            LootCount = result.LootCount;
            
            if (string.IsNullOrEmpty(name))
            {
                name = $"{result.name} (net loaded)";
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying == false)
            {

                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                AssetReference reference = new AssetReference(guid);

                if (string.IsNullOrEmpty(_loadKey))
                {
                    _loadKey = new(reference.RuntimeKey.ToString());
                    EditorUtility.SetDirty(this);
                }
                if (string.IsNullOrEmpty(Id))
                {
                    Id = name;
                    EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}
