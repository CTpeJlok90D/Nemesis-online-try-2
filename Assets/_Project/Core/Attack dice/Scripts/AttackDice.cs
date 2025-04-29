using UnityEngine;

namespace Core
{
    public static class AttackDice
    {
        private static Result[] Sides =
        {
            Result.Double,
            Result.One,
            Result.Adult,
            Result.Creeper,
            Result.Creeper,
            Result.Miss
        };

        public static Result Roll()
        {
            return Sides[Random.Range(0, Sides.Length)];
        }
        
        public enum Result
        {
            Double,
            One,
            Adult,
            Creeper,
            Miss
        }
    }
}
