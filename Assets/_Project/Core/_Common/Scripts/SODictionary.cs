using System.Collections.Generic;
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

        public TValue this[TKey key] => _assets.GetValueOrDefault(key, _errorValue);
    }
}