using System;
using Unity.Netcode;

namespace Core.Missions.Dealing
{
    [Serializable]
    public struct MissionsDealerConfiguration : INetworkSerializable
    {
        public Mission[] AvailableMissions;
        public int PersonalMissionsCount;
        public int CorporateMissionsCount;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AvailableMissions);
            serializer.SerializeValue(ref PersonalMissionsCount);
            serializer.SerializeValue(ref CorporateMissionsCount);
        }
    }
}
