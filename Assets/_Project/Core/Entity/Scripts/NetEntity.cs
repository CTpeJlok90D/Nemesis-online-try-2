using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Entities
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetEntity<T> : NetworkBehaviour where T : NetEntity<T>
    {
        private static readonly List<T> _instances = new();

        public delegate void SpawnedDelegate(T spawned);
        public static event SpawnedDelegate Spawned;
        public static event SpawnedDelegate Despawned;
        
        public static IReadOnlyList<T> Instances
        {
            get { return _instances.ToArray(); }
        }

        protected virtual void OnEnable()
        {
            _instances.Add((T)this);
            Spawned?.Invoke((T)this);
        }

        protected virtual void OnDisable()
        {
            _instances.Remove((T)this);
            Despawned?.Invoke((T)this);
        }
    }
}
