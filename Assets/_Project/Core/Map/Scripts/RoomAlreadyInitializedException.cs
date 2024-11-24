using System;

namespace Core.Maps
{
    public class RoomAlreadyInitializedException : Exception
    {
        public RoomAlreadyInitializedException() : base() { }
        public RoomAlreadyInitializedException(string message) : base(message) { }
    }
}