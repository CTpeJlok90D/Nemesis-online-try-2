using UnityEngine;

namespace Core
{
    public static class NoiseDice
    {
        public static Result Roll()
        {
            int intResult = Random.Range(0, 10+1);

            if (intResult == 10)
            {
                return Result.Silence;
            }

            if (intResult == 9)
            {
                return Result.Dangerous;
            }

            if (intResult >= 4)
            {
                intResult -= 4;
            }
            
            return (Result)intResult;
        } 
        
        public enum Result
        {
            Silence = -2,
            Dangerous = -1,
            One = 0,
            Two = 1,
            Three = 2,
            Four = 3,
        }
    }
}
