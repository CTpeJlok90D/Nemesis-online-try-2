using Core.TimeTracks;
using UnityEngine;

namespace Core.Scenarios.EnemiesPhase
{
    public class MoveTrackChapter : IChapter
    {
        private TimeTrack _timeTrack;
        
        public MoveTrackChapter(TimeTrack timeTrack)
        {
            _timeTrack =  timeTrack;
        }
        
        public void Begin()
        {
            if (_timeTrack.IsActive.Value)
            {
                _timeTrack.Current.Value--;
            }
            Ended?.Invoke(this);
        }

        public event IChapter.EndedListener Ended;
    }
}
