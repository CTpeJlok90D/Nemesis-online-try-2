using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetBehaviourReference<T> : NetworkVariable<NetworkObjectReference> where T : NetworkBehaviour
    {
        public delegate void ReferenceChangedListener(T oldValue, T newValue);

        private T _previousReference;

        public T Reference 
        {
            get
            {
                if (Value.TryGet(out NetworkObject networkObject))
                {
                    T result = networkObject.GetComponent<T>();
                    return result;
                }
                return default;
            }
            set
            {
                if (Value.TryGet(out NetworkObject netObject))
                {
                    _previousReference = netObject.GetComponent<T>();
                }
                else
                {
                    _previousReference = default;
                }

                if (value == null)
                {
                    Value = new NetworkObjectReference();
                    return;
                }
                Value = value.NetworkObject;
            }
        }

        public event OnValueChangedDelegate Changed;

        public event ReferenceChangedListener ReferenceChanged;

        public NetBehaviourReference()
        {
            OnValueChanged = OnValueChange;
        }

        public NetBehaviourReference(T value = default,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(value.NetworkObject, readPerm, writePerm)
        {
            OnValueChanged = OnValueChange;
        }

        private void OnValueChange(NetworkObjectReference previousValue, NetworkObjectReference newValue)
        {
            Changed?.Invoke(previousValue, newValue);
            ReferenceChanged?.Invoke(_previousReference, Reference);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Changed?.Invoke(Value, Value);
            ReferenceChanged?.Invoke(_previousReference, Reference);
        }
    }
}
