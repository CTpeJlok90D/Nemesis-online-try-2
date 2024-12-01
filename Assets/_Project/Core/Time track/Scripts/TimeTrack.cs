using System;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.TimeTrack
{
    [Icon("Assets/_Project/Core/Time track/Editor/icons8-timer-96.png")]
    public class TimeTrack : NetworkBehaviour
    {
        public delegate void TimeOutListener(TimeTrack sender);

        [field: SerializeField] public int Max { get; private set; }
        [field: SerializeField] public TimeTrackType Type { get; private set; }

        public NetVariable<int> Current { get; private set; }

        public event TimeOutListener TimeOut;

        protected virtual void Awake()
        {
            Current = new(Max);
        }

        protected virtual void OnEnable()
        {
            Current.Changed += OnCurrentChange;
        }

        protected virtual void OnDisable()
        {
            Current.Changed -= OnCurrentChange;
        }

        private void OnCurrentChange(int previousValue, int newValue)
        {
            if (newValue < 0)
            {
                throw new IndexOutOfRangeException("Time track can't be negative");
            }

            if (newValue > Max)
            {
                throw new IndexOutOfRangeException("Time track can't be bigger than max");
            }

            if (newValue == 0)
            {
                TimeOut?.Invoke(this);
            }
        }
    }
}
