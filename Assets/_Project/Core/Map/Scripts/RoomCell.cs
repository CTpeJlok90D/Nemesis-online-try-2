using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Custom;
using Core.Maps.IntellegenceTokens;
using System.Collections.Generic;
using AYellowpaper;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-room-96.png")]
    public class RoomCell : NetworkBehaviour
    {
        public const int NOISE_CONTAINERS_COUNT = 4;

        [field: SerializeField] private RoomType _roomContent;

        [field: SerializeField] public int Number { get; private set; } = 1;

        [field: SerializeField] public int Layer { get; private set; } = 1;

        [field: SerializeField] public bool GenerateIntellegenceToken { get; private set; } = true;

        [field: SerializeField] private InterfaceReference<INoiseContainer>[] _linkedTunnels = new InterfaceReference<INoiseContainer>[NOISE_CONTAINERS_COUNT];

        private NetworkList<NetworkObjectReference> _roomContentsNet;

        private RoomContent[] _roomContents;

        public NetVariable<IntelegenceToken> IntellegenceTokenNet { get; private set; }
        
        private NetVariable<RoomType> _roomTypeNet;

        private NetVariable<bool> _isExplored;

        public NetVariable<bool> IsInitialized { get; private set; }

        public RoomType Type => _roomTypeNet.Value;

        public IReadOnlyReactiveField<bool> IsExplored => _isExplored;

        public IReadOnlyCollection<RoomContent> RoomContents => _roomContents;

        public IReadOnlyCollection<INoiseContainer> NoiseContainers => _linkedTunnels.Select(x => x.Value).ToArray();

        public event IReadOnlyReactiveField<RoomType>.ChangedListener TypeChanged
        {
            add => _roomTypeNet.Changed += value;
            remove => _roomTypeNet.Changed -= value;
        }

        private void Awake()
        {
            if (_linkedTunnels.Contains(null))
            {
                throw new System.Exception("Room cell contains null noiseContainer");
            }

            if (_linkedTunnels.Length != NOISE_CONTAINERS_COUNT)
            {
                throw new System.Exception($"Every room cell must have {NOISE_CONTAINERS_COUNT} tunnels");
            }

            IntellegenceTokenNet = new();
            _roomTypeNet = new();
            IsInitialized = new();
            _roomContentsNet = new();
            _isExplored = new();
            _roomContents = Array.Empty<RoomContent>();

            _roomContentsNet.OnListChanged += OnListChange;
        }

        public INoiseContainer[] GetPassagesTo(RoomCell other)
        {
            INoiseContainer[] noiseContainers = NoiseContainers.Intersect(other.NoiseContainers).ToArray();
            return noiseContainers;
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.IsServer == false)
            {
                return;
            }

            _roomTypeNet.Value = _roomContent;
        }

        public RoomCell Init(RoomType roomContent)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can initialize rooms");
            }

            if (IsInitialized.Value)
            {
                throw new RoomAlreadyInitializedException();
            }

            _roomTypeNet.Value = roomContent;
            IsInitialized.Value = true;
            return this;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _roomContentsNet.OnListChanged -= OnListChange;
        }

        private void OnListChange(NetworkListEvent<NetworkObjectReference> changeEvent)
        {
            List<RoomContent> contents = new();
            foreach (NetworkObjectReference reference in _roomContentsNet.ToEnumerable())
            {
                if (reference.TryGet(out NetworkObject result))
                {
                    RoomContent content = result.GetComponent<RoomContent>();
                    contents.Add(content);
                    content.Owner = this;
                }
            }

            _roomContents = contents.ToArray();
        }

        public void AddContent(RoomContent content)
        {
            RoomCell oldRoom = content.Owner;
            oldRoom?.RemoveContent(content);
            _roomContentsNet.Add(content.gameObject);

            if (oldRoom != null)
            {
                OnMove_RPC(content.NetworkObject, oldRoom.NetworkObject);
            }
            else
            {
                OnMove_RPC(content.NetworkObject, new());
            }
        }

        public void Explore()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can explore rooms");
            }
            
            _isExplored.Value = true;
        }

        [Rpc(SendTo.Everyone)]
        private void OnMove_RPC(NetworkObjectReference roomContent, NetworkObjectReference oldRoomNetReference)
        {
            RoomCell oldRoom = null;

            if (oldRoomNetReference.TryGet(out NetworkObject roomCellNetObject))
            {
                oldRoom = roomCellNetObject.GetComponent<RoomCell>();
            }

            NetworkObject roomContentNetObject = roomContent;
            RoomContent content = roomContentNetObject.GetComponent<RoomContent>();

            content.Owner = this;

            IRoomOwnerChangedHandler[] hadlers = content.GetComponents<IRoomOwnerChangedHandler>();
            foreach (IRoomOwnerChangedHandler changedHandler in hadlers)
            {
                changedHandler.OnRoomMove(oldRoom, this);
            }
        }

        private void RemoveContent(RoomContent content)
        {
            _roomContentsNet.Remove(content.gameObject);
        }

        public override string ToString()
        {
            return $"{_roomTypeNet.Value}:{Number}";
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
                    EditorGUILayout.ObjectField("Room: " ,RoomCell._roomTypeNet.Value, typeof(RoomType), false);
                    EditorGUILayout.ObjectField("Intellegence token: " ,RoomCell.IntellegenceTokenNet.Value, typeof(IntelegenceToken), false);
                    
                    if (RoomCell.RoomContents != null)
                    {
                        foreach (RoomContent content in RoomCell.RoomContents)
                        {
                            EditorGUILayout.ObjectField(content, typeof(RoomContent), false);
                        }
                    }

                    GUI.enabled = true;

                }
            }
        }
#endif
    }
}