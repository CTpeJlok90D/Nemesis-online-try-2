using Core.Maps;
using Core.Maps.IntellegenceTokens;
using Core.PlayerTablets;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token action/Add room content intellegence token action")]
    public class AddRoomContentIntellegenceTokenAction : IntelegenceTokenAction
    {
        [SerializeField] private RoomContent _contentToAdd;

        public override void Execute(RoomCell selfRoom, RoomCell roomExecutorCameFrom, PlayerTablet executor)
        {
            RoomContent content = Instantiate(_contentToAdd);
            content.NetworkObject.Spawn();
            selfRoom.AddContent(content);
        }
    }
} 