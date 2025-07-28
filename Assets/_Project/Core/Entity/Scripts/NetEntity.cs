using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Core.Entities
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetEntity<T> : NetworkBehaviour where T : NetEntity<T>
    {
        private static readonly List<T> _instances = new();

        public delegate void SpawnedDelegate(T spawned);
        public delegate void DespawnedDelegate(T despawned);
        public static event SpawnedDelegate Spawned;
        public static event DespawnedDelegate Despawned;
        
        public static IReadOnlyList<T> Instances
        {
            get { return _instances.ToArray(); }
        }

        public static T Instance
        {
            get { return _instances.First(); }
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
