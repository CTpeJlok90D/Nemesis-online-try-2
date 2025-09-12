using Core.Maps;
using Core.Maps.IntellegenceTokens;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token action/Force dice room intellegence token")]
    public class ForceDiceRoomIntellegenceToken : IntelegenceTokenAction, IMoveNoiseBlocker
    {
        [SerializeField] private NoiseDice.Result _forceResult;

        public override void Execute(RoomCell selfRoom, RoomCell roomExecutorCameFrom, PlayerTablet executor)
        {
            _ = Ship.Instance.NoiseInRoom(selfRoom, _forceResult);
        }
    }
}