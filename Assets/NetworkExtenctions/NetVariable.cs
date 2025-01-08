using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    public class NetVariable<T> : NetworkVariable<T>
    {
        private List<OnValueChangedDelegate> _listeners = new();

        public event OnValueChangedDelegate Changed
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
            foreach (OnValueChangedDelegate listener in _listeners.ToArray())
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
