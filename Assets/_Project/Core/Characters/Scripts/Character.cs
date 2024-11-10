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
        [field: SerializeField] public AssetReferenceT<Character> AddessablePath { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject CharacterPawn { get; private set; }
        [field: SerializeField] public AssetReferenceT<Sprite> CharacterAvatar { get; private set; }
        [field: SerializeField] public AssetReferenceT<Sprite> CharacterBackground { get; private set; }

        public bool Equals(Character other)
        {
            return
                AddessablePath.Equals(other.AddessablePath) &&
                CharacterPawn.Equals(other.CharacterPawn) &&
                CharacterAvatar.Equals(other.CharacterAvatar) &&
                CharacterBackground.Equals(other.CharacterBackground);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            FixedString64Bytes key = "";

            if (serializer.IsWriter)
            {
                key = AddessablePath.RuntimeKey.ToString();
            }

            serializer.SerializeValue(ref key);

            if (serializer.IsReader)
            {
                AsyncOperationHandle<Character> handle = Addressables.LoadAssetAsync<Character>(key.ToString());
                handle.Completed += handle => 
                {
                    name = handle.Result.name + "(Clone)";
                    AddessablePath = handle.Result.AddessablePath;
                    CharacterPawn = handle.Result.CharacterPawn;
                    CharacterAvatar = handle.Result.CharacterAvatar;
                    CharacterBackground = handle.Result.CharacterBackground;
                };
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
            AddessablePath = new(GUID);
        }

        [CustomEditor(typeof(Character))]
        private class CEditor : Editor
        {
            private Character character => target as Character;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                GUILayout.Label("Key: " + character.AddessablePath.RuntimeKey.ToString());
                GUILayout.Label("Key type: " + character.AddessablePath.RuntimeKey.GetType().FullName);
                if (GUILayout.Button("Load"))
                {
                    AsyncOperationHandle<Character> handle = Addressables.LoadAssetAsync<Character>(character.AddessablePath.RuntimeKey);
                    handle.Completed += completed => 
                    {
                        Debug.Log("Loaded!", completed.Result);
                    };
                }
            }
        }
#endif
    }   
}