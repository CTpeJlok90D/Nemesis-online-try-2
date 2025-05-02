using UnityEngine;

namespace Core.EventsDecks
{
    public abstract class EventCardAction : ScriptableObject
    {
        public abstract void Execute();
    }
}
