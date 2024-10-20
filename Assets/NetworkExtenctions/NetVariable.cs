using System;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetVariable<T> : NetworkVariable<T>
    {
        public event OnValueChangedDelegate ValueChanged;

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

        private void OnValueChange(T previousValue, T newValue)
        {
            try
            {
                ValueChanged?.Invoke(previousValue, newValue);
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }
    }
}
