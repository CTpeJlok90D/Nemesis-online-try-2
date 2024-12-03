using System;
using UnityEngine;

namespace Core.Scenarios
{
    public class Delay : IChapter
    {
        private float _time;
        
        public Delay(float time = 1f)
        {
            _time = time;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            Wait();
        }

        private async void Wait()
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(_time);
                Ended?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Ended?.Invoke(this);
            }
        }
    }
}
