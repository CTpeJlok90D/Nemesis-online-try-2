using System.Collections.Generic;
using Unity.Netcode;

namespace Core.Entity
{
    public abstract class NetEntity<T> : NetworkBehaviour
    {
        private static List<T> _instances = new();
        
        public static IReadOnlyList<T> Instances
        {
            get { return _instances; }
        }

        protected abstract T Instance { get; }

        protected virtual void OnEnable()
        {
            _instances.Add(Instance);
        }

        protected virtual void OnDisable()
        {
            _instances.Remove(Instance);
        }
    }
}
