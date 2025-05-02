using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetVariable<T> : NetworkVariable<T>, IReadOnlyReactiveField<T>
    {
        private List<IReadOnlyReactiveField<T>.ChangedListener> _listeners = new();
        
        public event IReadOnlyReactiveField<T>.ChangedListener Changed
        {
            add => _listeners.Add(value);
            remove => _listeners.Remove(value);
        }

        public NetVariable() : base()
        {
            OnValueChanged = OnValueChange;
        }

        public NetVariable(
            T value = default,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(value, readPerm, writePerm)
        {
            OnValueChanged = OnValueChange;
        }

        protected virtual void OnValueChange(T previousValue, T newValue)
        {
            foreach (IReadOnlyReactiveField<T>.ChangedListener listener in _listeners.ToArray())
            {
                try
                {
                    listener?.Invoke(previousValue, newValue);   
                }
                catch (Exception e) 
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
