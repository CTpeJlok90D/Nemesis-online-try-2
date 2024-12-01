using System;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    public class NetVariable<T> : NetworkVariable<T>
    {
        public event OnValueChangedDelegate Changed;

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
            try
            {
                Changed?.Invoke(previousValue, newValue);
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }
    }
}
