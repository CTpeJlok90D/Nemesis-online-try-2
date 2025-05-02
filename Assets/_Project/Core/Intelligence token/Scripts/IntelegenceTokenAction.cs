using UnityEngine;

namespace Core.Maps.IntellegenceTokens
{
    [Icon("Assets/_Project/Core/Map/Editor/intellegence-token-action.png")]
    public abstract class IntelegenceTokenAction : ScriptableObject
    {
        [SerializeField] private int _cost;

        public int Cost => _cost;
    }
}
