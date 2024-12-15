using System;
using Core.Common;
using Core.Maps;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace View.Rooms
{
    public class RoomPrefabSpawner : MonoBehaviour
    {
        [SerializeField] private RoomCell _linkedRoomCell;
        [Inject] private GameObjectsDictionary _roomsDictionary;

        private GameObject _roomViewInstance;

        private void OnEnable()
        {
            if (didStart)
            {
                Start();
            }
        }

        private void Start()
        {
            _linkedRoomCell.TypeChanged += OnValueChange;
            UpdateRoom();
        }

        private void OnDisable()
        {
            _linkedRoomCell.TypeChanged += OnValueChange;
        }

        
        private void OnValueChange(RoomType previousValue, RoomType newValue)
        {
            UpdateRoom();
        }

        private void UpdateRoom()
        {
            if (_roomViewInstance != null)
            {
                Destroy(_roomViewInstance);
            }

            if (_linkedRoomCell.Type == null)
            {
                return;
            }

            AssetReferenceGameObject assetReferenceGameObject = _roomsDictionary[_linkedRoomCell.Type.Id];
            AsyncOperationHandle<GameObject> instantiateLoadHandle = assetReferenceGameObject.InstantiateAsync(transform);
            instantiateLoadHandle.Completed += (handle) => 
            {
                _roomViewInstance = handle.Result;
            };
        }
    }
}
