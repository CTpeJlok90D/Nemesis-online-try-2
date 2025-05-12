using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Entities
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetEntity<T> : NetworkBehaviour where T : NetEntity<T>
    {
        private static readonly List<T> _instances = new();
        
        public static IReadOnlyList<T> Instances
        {
            get { return _instances; }
        }

        protected virtual void OnEnable()
        {
            _instances.Add((T)(object)this);
        }

        protected virtual void OnDisable()
        {
            _instances.Remove((T)(object)this);
        }
    }
}
