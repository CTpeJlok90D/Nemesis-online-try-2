using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class ReactiveField<T> : IReadOnlyReactiveField<T>
    {
        [SerializeField] private T _value = default;
        public event IReadOnlyReactiveField<T>.ChangedListener Changed;

        public ReactiveField(T value)
        {
            _value = value;
        }

        public ReactiveField()
        {
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                T oldValue = _value;
                _value = value;
                Changed?.Invoke(oldValue, _value);
            }
        }
    }
}