using UnityEngine;

namespace Core.EventsDeck
{
    public abstract class EventCardAction : ScriptableObject
    {
        public abstract void Execute();
    }
}
