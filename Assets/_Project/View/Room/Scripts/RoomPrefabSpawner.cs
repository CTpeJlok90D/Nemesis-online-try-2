using Core.Common;
using Core.Maps;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace View.Rooms
{
    [DefaultExecutionOrder(5)]
    public class RoomPrefabSpawner : MonoBehaviour
    {
        [SerializeField] private RoomCell _linkedRoomCell;
        [SerializeField] private GameObjectByID _roomsDictionary;
        [SerializeField] private GameObjectByNumber _unexploredRoomsView;

        private GameObject _roomViewInstance;

        private void OnEnable()
        {
            _linkedRoomCell.TypeChanged += OnValueChange;
            _linkedRoomCell.IsExplored.Changed += OnIsExploredChange;
            UpdateRoom();
        }

        private void OnDisable()
        {
            _linkedRoomCell.TypeChanged += OnValueChange;
            _linkedRoomCell.IsExplored.Changed -= OnIsExploredChange;
        }

        private void OnIsExploredChange(bool oldvalue, bool newvalue) => UpdateRoom();
        private void OnValueChange(RoomType previousValue, RoomType newValue) => UpdateRoom();
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

            if (_linkedRoomCell.IsExplored.Value)
            {
                _ = SpawnExploredRoomView();
            }
            else
            {
                _ = SpawnUnexploredRoomView();
            }
        }

        private async UniTask SpawnUnexploredRoomView()
        {
            AssetReferenceGameObject assetReferenceGameObject = _unexploredRoomsView[_linkedRoomCell.Layer];
            AsyncOperationHandle<GameObject> instantiateLoadHandle = assetReferenceGameObject.InstantiateAsync(transform);
            await instantiateLoadHandle.ToUniTask();
            _roomViewInstance = instantiateLoadHandle.Result;
        }

        private async UniTask SpawnExploredRoomView()
        {
            AssetReferenceGameObject assetReferenceGameObject = _roomsDictionary[_linkedRoomCell.Type.Id];
            AsyncOperationHandle<GameObject> instantiateLoadHandle = assetReferenceGameObject.InstantiateAsync(transform);
            await instantiateLoadHandle.ToUniTask();
            _roomViewInstance = instantiateLoadHandle.Result;
        }
    }
}
