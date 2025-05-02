using System;
using Core.Entity;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.TimeTracks
{
    [Icon("Assets/_Project/Core/Time track/Editor/icons8-timer-96.png")]
    public class TimeTrack : NetEntity<TimeTrack>
    {
        public delegate void TimeOutListener(TimeTrack sender);

        [field: SerializeField] public int Max { get; private set; }
        [field: SerializeField] public TimeTrackType Type { get; private set; }
        [SerializeField] public bool _isActiveByDefault = true;

        public NetVariable<int> Current { get; private set; }
        public NetVariable<bool> IsActive { get; private set; }

        public event TimeOutListener TimeOut;
        
        protected virtual void Awake()
        {
            Current = new(Max);
            IsActive = new(_isActiveByDefault);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Current.Changed += OnCurrentChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Current.Changed -= OnCurrentChange;
        }

        private void OnCurrentChange(int previousValue, int newValue)
        {
            if (newValue < 0)
            {
                Current.Value = 0;
                throw new IndexOutOfRangeException("Time track can't be negative");
            }

            if (newValue > Max)
            {
                Current.Value = Max;
                throw new IndexOutOfRangeException("Time track can't be bigger than max");
            }

            if (newValue == 0)
            {
                TimeOut?.Invoke(this);
            }
        }
    }
}
