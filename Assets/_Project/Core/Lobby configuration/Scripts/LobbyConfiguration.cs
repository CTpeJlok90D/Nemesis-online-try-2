using System;
using Unity.Netcode;

namespace Core.Lobbies
{
    [Serializable]
    public struct LobbyConfiguration : INetworkSerializeByMemcpy
    {
        public int PlayersCount;    
    }
}
