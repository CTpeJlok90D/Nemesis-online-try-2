using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.Common
{
    [Icon("Assets/_Project/Core/_Common/Editor/icons8-book-96.png")]
    public class SODictionary<TKey, TValue> : ScriptableObject
    {
        [SerializedDictionary("ID", "RESULT")]
        [SerializeField] private SerializedDictionary<TKey, TValue> _assets;
        [SerializeField] private TValue _errorValue;

        public TValue this[TKey key]
        {
            get
            {
                if (_assets.TryGetValue(key, out TValue assetReferenceGameObject))
                {
                    return assetReferenceGameObject;
                }
                return _errorValue;
            }
        }
    }
}