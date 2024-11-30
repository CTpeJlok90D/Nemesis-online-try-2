using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Custom;
using Core.Map.IntellegenceTokens;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-room-96.png")]
    public class RoomCell : NetworkBehaviour
    {
        [field: SerializeField] private RoomContent _roomContent;

        [field: SerializeField] public int Number { get; private set; } = 1;

        [field: SerializeField] public int Layer { get; private set; } = 1;

        [field: SerializeField] public bool GenerateIntellegenceToken { get; private set; } = true;

        public NetVariable<IntelegenceToken> IntellegenceTokenNet { get; private set; }
        
        private NetVariable<RoomContent> _roomContentNet;

        public NetVariable<bool> IsInitialized { get; private set; }

        public RoomContent RoomContent => _roomContentNet.Value;

        private void Awake()
        {
            IntellegenceTokenNet = new();
            _roomContentNet = new();
            IsInitialized = new();
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.IsServer == false)
            {
                return;
            }

            _roomContentNet.Value = _roomContent;
        }

        public RoomCell Init(RoomContent roomContent)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can initialize rooms");
            }

            if (IsInitialized.Value)
            {
                throw new RoomAlreadyInitializedException();
            }

            _roomContentNet.Value = roomContent;
            IsInitialized.Value = true;
            return this;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(RoomCell))]
        private class CEditor : Editor
        {
            private RoomCell RoomCell => target as RoomCell;
            public override void OnInspectorGUI()
            {
                if (Application.isPlaying == false)
                {
                    base.OnInspectorGUI();
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                    EditorGUILayout.ObjectField("Room: " ,RoomCell._roomContentNet.Value, typeof(RoomContent), false);
                    EditorGUILayout.ObjectField("Intellegence token: " ,RoomCell.IntellegenceTokenNet.Value, typeof(IntelegenceToken), false);
                    GUI.enabled = true;
                }
            }
        }
#endif
    }
}