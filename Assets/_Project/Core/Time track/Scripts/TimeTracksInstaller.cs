using UnityEngine;
using Zenject;

namespace Core.TimeTrack
{
    public class TimeTracksInstaller : MonoInstaller
    {
        [SerializeField] private TimeTrack[] _timeTracks;

        public override void InstallBindings()
        {
            foreach (TimeTrack track in _timeTracks)
            {
                Container
                    .Bind<TimeTrack>()
                    .WithId(track.Type)
                    .FromInstance(track);
            }
        }
    }
}
