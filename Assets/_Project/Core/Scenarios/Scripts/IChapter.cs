using UnityEngine;

namespace Core.Scenarios
{
    public interface IChapter
    {
        public delegate void EndedListener(IChapter sender);

        public void Begin();

        public event EndedListener Ended;
    }
}
