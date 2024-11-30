using System;

namespace Core.Maps.Generation
{
    public class MapGenerationException : Exception
    {
        public MapGenerationException() : base() {}

        public MapGenerationException(string message) : base(message) {}
    }
}
