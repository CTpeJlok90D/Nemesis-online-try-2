using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetBehaviourReference<T> : NetworkVariable<NetworkObjectReference>, IReadOnlyReactiveField<T> where T : NetworkBehaviour
    {
        public delegate void ReferenceChangedListener(T oldValue, T newValue);

        private T _previousReference;

        public T Value 
        {
            get
            {
                if (base.Value.TryGet(out NetworkObject networkObject))
                {
                    T result = networkObject.GetComponent<T>();
                    return result;
                }
                return default;
            }
            set
            {
                if (base.Value.TryGet(out NetworkObject netObject))
                {
                    _previousReference = netObject.GetComponent<T>();
                }
                else
                {
                    _previousReference = default;
                }

                if (value == null)
                {
                    base.Value = new NetworkObjectReference();
                    return;
                }
                base.Value = value.NetworkObject;
            }
        }
        
        public event IReadOnlyReactiveField<T>.ChangedListener Changed;

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
            Changed?.Invoke(_previousReference, Value);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Changed?.Invoke(Value, Value);
        }
    }
}
