using System;
using Unity.Netcode;

namespace Core.PlayerActions
{
    public interface IPayment : INetworkSerializable, IEquatable<IPayment>
    {
        public int Worth { get; }

        public void Pay();
    }
}