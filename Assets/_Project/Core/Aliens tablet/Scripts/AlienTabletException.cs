using System;

namespace Core.AliensTablets
{
    public class AlienTabletAlreadyInitialized : Exception
    {
        public AlienTabletAlreadyInitialized() : base() {}

        public AlienTabletAlreadyInitialized(string message) : base(message) {}
    }
}
