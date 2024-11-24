using UnityEngine;

namespace Core.Scenarios
{
    public struct Scenario
    {
        public Scenario(IChapter[] chapters)
        {
            Chapters = chapters;
        }

        public IChapter[] Chapters;
    }
}
