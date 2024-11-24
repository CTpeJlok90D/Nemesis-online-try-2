using System;

namespace Core.TabletopRandom
{
    public class BagException : Exception
    {
        public BagException() : base(){}
        public BagException(string message) : base(message){}
    }
}
