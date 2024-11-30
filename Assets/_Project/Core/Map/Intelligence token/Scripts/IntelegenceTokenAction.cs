using Core.PlayerActions;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.Map.IntellegenceTokens
{
    [Icon("Assets/_Project/Core/Map/Editor/intellegence-token-action.png")]
    public abstract class IntelegenceTokenAction : ScriptableObject, IPlayerAction
    {
        [SerializeField] private int _cost;

        public int Cost => _cost;

        public abstract void Execute(PlayerTablet execotor, IPayment[] payments, GameObject[] targets);
    }
}
