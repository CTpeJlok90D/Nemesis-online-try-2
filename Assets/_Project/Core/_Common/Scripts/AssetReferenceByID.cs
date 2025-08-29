using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Common
{
    public abstract class AssetReferenceByID<TKey, TValue> : SODictionary<TKey, AssetReferenceT<TValue>> where TValue : Object
    {
        private static Dictionary<TKey, TValue> _avatarLoadedValues = new();
        
        public async UniTask<TValue> LoadAsset(TKey key)
        {
            AssetReferenceT<TValue> result = this[key];

            if (_avatarLoadedValues.TryGetValue(key, out TValue value))
            {
                await UniTask.WaitUntil(() => _avatarLoadedValues[key] != null);
                
                return _avatarLoadedValues[key];
            }

            AssetReferenceT<TValue> avatarReference = this[key];
            AsyncOperationHandle<TValue> assetReferenceHandle = Addressables.LoadAssetAsync<TValue>(avatarReference.RuntimeKey);
            
            _avatarLoadedValues.Add(key, null);
            await assetReferenceHandle.ToUniTask();
            _avatarLoadedValues[key] = assetReferenceHandle.Result;
            
            return assetReferenceHandle.Result;
        }
    }
}