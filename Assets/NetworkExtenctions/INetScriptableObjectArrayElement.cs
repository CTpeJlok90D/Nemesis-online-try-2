using System;

namespace Unity.Netcode.Custom
{
    public interface INetScriptableObjectArrayElement<T> where T : UnityEngine.Object, INetworkSerializable, IEquatable<T>
    {
        public NetScriptableObject<T> Net { get; }
    }
}
