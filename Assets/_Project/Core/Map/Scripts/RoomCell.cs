using Unity.Netcode;
using UnityEngine;

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
        
        private NetworkVariable<RoomContent> _roomContentNet;

        private NetworkVariable<bool> _isInitialized;

        public RoomContent RoomContent => _roomContentNet.Value;

        private void Awake()
        {
            _roomContentNet = new();
            _isInitialized = new();
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

            if (_isInitialized.Value)
            {
                throw new RoomAlreadyInitializedException();
            }

            _roomContentNet.Value = roomContent;
            _isInitialized.Value = true;
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
                    GUI.enabled = true;
                }
            }
        }
#endif
    }
}