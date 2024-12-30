using System;
using AYellowpaper.SerializedCollections;
using Core.Characters;
using Core.Map.CharacterPawns;

namespace Core.Scenarios
{
    [Serializable]
    public struct PawnPlacerConfig
    {
        [SerializedDictionary("ID", "Pawn PREFAB")]
        public SerializedDictionary<string, CharacterPawn> PawnsForCharacters;
    }
}
