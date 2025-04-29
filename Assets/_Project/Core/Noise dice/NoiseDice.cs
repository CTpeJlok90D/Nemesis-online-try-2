using UnityEngine;

namespace Core
{
    public static class NoiseDice
    {
        private static Result[] _sides =
        {
            Result.Dangerous,
            Result.Silence,
            Result.Four,
            Result.Four,
            Result.Three,
            Result.Three,
            Result.Two,
            Result.Two,
            Result.One,
            Result.One,
        };
        
        public static Result Roll()
        {
            return _sides[Random.Range(0, _sides.Length)];
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
