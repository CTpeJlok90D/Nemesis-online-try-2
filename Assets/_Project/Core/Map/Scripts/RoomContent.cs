using Core.Entities;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using Zenject;

namespace Core.Maps
{
    [RequireComponent(typeof(NetworkObject))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-box-100.png")]
    public class RoomContent : NetEntity<RoomContent>
    {
        [Inject] private Ship _ship;
        
        private NetBehaviourReference<RoomCell> _ownerNet { get; set; }

        public RoomCell Owner
        {
            get
            {
                return _ownerNet.Value;
            }
            internal set
            {
                _ownerNet.Value = value;
            }
        }

        public event IReadOnlyReactiveField<RoomCell>.ChangedListener OwnerChanged
        {
            add => _ownerNet.Changed += value;
            remove => _ownerNet.Changed -= value;
        }

        public delegate void DespawnedHandler(RoomContent sender);
        public new static event DespawnedHandler Despawned;

        private void Awake()
        {
            _ownerNet = new();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Despawned?.Invoke(this);
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(RoomContent))]
        private class CEditor : Editor
        {
            public RoomContent RoomContent => target as RoomContent;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (Application.isPlaying == false)
                {
                    return;
                }
                
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Room cell", RoomContent._ownerNet.Value, typeof(RoomCell), false);
                GUI.enabled = true;
            }
        }
#endif
    }
}
