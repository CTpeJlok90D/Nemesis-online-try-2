using Core.Maps;
using Core.Maps.IntellegenceTokens;
using Core.PlayerTablets;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token action/Add slime to character token action")]
    public class AddSlimeToCharacterTokenAction : IntelegenceTokenAction
    {
        public override void Execute(RoomCell selfRoom, RoomCell roomExecutorCameFrom, PlayerTablet executor)
        {
            executor.AddTag(PlayerTag.Slime);
        }
    }
}