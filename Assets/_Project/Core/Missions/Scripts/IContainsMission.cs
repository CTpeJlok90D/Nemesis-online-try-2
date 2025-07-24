using System;

namespace Core.Missions
{
    public interface IContainsMission
    {
        public Mission Mission { get; }

        public event ChangedDelegate Changed;
        public delegate void ChangedDelegate(Mission oldMission, Mission newMission);
    }
}