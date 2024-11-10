using System;
using Unity.Netcode;

namespace Core.LoadObservers
{
    [Serializable]
    public struct LoadStatus : INetworkSerializeByMemcpy, IEquatable<LoadStatus>
    {
        public ulong PlayerID;
        public bool IsReady;

        public bool Equals(LoadStatus other)
        {
            return
                other.PlayerID == PlayerID && 
                other.IsReady == IsReady;
        }
    }
}