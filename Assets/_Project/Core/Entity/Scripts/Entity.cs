using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class Entity<T> : MonoBehaviour where T : Entity<T>
    {
        private static List<T> _instances = new();
        
        public static IReadOnlyList<T> Instances
        {
            get { return _instances; }
        }

        protected virtual void OnEnable()
        {
            _instances.Add((T)this);
        }

        protected virtual void OnDisable()
        {
            _instances.Remove((T)this);
        }
    }
}
