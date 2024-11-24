using System;

namespace Core.Scenarios
{
    public class ScenarioException : Exception
    {
        public ScenarioException() : base() {}
        public ScenarioException(string message) : base(message){}
    }
}
