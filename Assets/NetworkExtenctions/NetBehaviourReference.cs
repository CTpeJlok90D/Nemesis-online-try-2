using System;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetBehaviourReference<T> : NetworkVariable<NetworkObjectReference> where T : NetworkBehaviour
    {
        public delegate void ReferenceChangedListener(T oldValue, T newValue);

        private T _reference;

        public T Reference 
        {
            get
            {
                return _reference;
            }
            set
            {
                base.Value = value.NetworkObject;
            }
        }

        public event OnValueChangedDelegate Changed;

        public event ReferenceChangedListener ReferenceChanged;

        public NetBehaviourReference() : base()
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
            try
            {
                Changed?.Invoke(previousValue, newValue);

                T previousReference = null;
                T newReference = null;

                if (previousValue.TryGet(out NetworkObject previousObject))
                {
                    previousReference = previousObject.GetComponent<T>();
                }

                if (newValue.TryGet(out NetworkObject newObject))
                {
                    newReference = newObject.GetComponent<T>();
                }

                _reference = newReference;
                ReferenceChanged?.Invoke(previousReference, newReference);
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }
    }
}
