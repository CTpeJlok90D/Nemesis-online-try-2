using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Netcode.Custom
{
    [Serializable]
    public static class NetObjectListExcentions
    {
        public static IEnumerable<T> ToEnumerable<T>(this NetworkList<T> list) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < list.Count; i++)
            {
                yield return list[i];
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this NetworkList<NetworkObjectReference> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].TryGet(out NetworkObject networkObject))
                {
                    T result = networkObject.GetComponent<T>();
                    yield return result;
                }
            }
        }
    }
}
