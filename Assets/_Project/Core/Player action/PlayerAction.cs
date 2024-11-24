using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;

namespace Core.PlayerActions
{
    public interface IPlayerAction : INetworkSerializable
    {
        public int Cost { get; }
        public void Execute(PlayerTablet execoter, IPayment[] payments, GameObject[] targets);
    }
}