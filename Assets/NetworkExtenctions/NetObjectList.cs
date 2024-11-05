using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Netcode.Custom
{
    public class NetObjectList<T> : NetworkList<NetworkObjectReference>, IEnumerable, IEnumerable<T> where T : NetworkBehaviour
    {
        public NetObjectList() : base()
        {

        }

        public NetObjectList(
            IEnumerable<T> values = null, 
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : 
                base(values.Select(x => new NetworkObjectReference(x.NetworkObject)), readPerm, writePerm)
        {

        }

        public void Add(T value)
        {
            Add(value.NetworkObject);
        }

        public void Remove(T value)
        {
            Remove(value.NetworkObject);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                NetworkObject networkObject = this[i];
                T result = networkObject.GetComponent<T>();
                yield return result;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                NetworkObject networkObject = this[i];
                T result = networkObject.GetComponent<T>();
                yield return result;
            }
        }
    }
}
