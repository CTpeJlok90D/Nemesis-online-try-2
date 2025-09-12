using Core.PlayerTablets;
using UnityEngine;

namespace Core.Maps.IntellegenceTokens
{
    [Icon("Assets/_Project/Core/Map/Editor/intellegence-token-action.png")]
    public abstract class IntelegenceTokenAction : ScriptableObject
    {
        public abstract void Execute(RoomCell selfRoom, RoomCell roomExecutorCameFrom, PlayerTablet executor);
    }
}
