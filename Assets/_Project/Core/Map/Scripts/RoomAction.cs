using UnityEngine;
using Core.PlayerActions;
using Core.PlayerTablets;
using Unity.Netcode.Custom;

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-room-action-96.png")]
    public abstract class RoomAction : NetScriptableObject, IPlayerAction
    {
        [SerializeField] private int _cost = 2;
        public int Cost => _cost;

        public abstract void Execute(PlayerTablet execoter, IPayment[] payments, GameObject[] targets);
    }
}
