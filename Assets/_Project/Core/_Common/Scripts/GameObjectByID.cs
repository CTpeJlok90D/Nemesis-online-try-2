using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Common
{
    [CreateAssetMenu(menuName = "Game objects dictionary")]
    [Icon("Assets/_Project/Core/_Common/Editor/icons8-book-96.png")]
    public class GameObjectsDictionary : ScriptableObject
    {
        [SerializedDictionary("ID", "PREFAB")]
        [SerializeField] private SerializedDictionary<string, AssetReferenceGameObject> _assets;
        [SerializeField] private AssetReferenceGameObject _errorValue;

        public AssetReferenceGameObject this[string key]
        {
            get
            {
                if (_assets.TryGetValue(key, out AssetReferenceGameObject assetReferenceGameObject))
                {
                    return assetReferenceGameObject;
                }
                return _errorValue;
            }
        }
    }
}