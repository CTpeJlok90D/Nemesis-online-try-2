using Core.PlayerActions;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.Map.IntellegenceTokens
{
    [CreateAssetMenu(menuName = "Game/Maps/Actions/Intelegence token test action")]
    public class TestIntellegenceToken : IntelegenceTokenAction
    {
        public override void Execute(PlayerTablet execotor, IPayment[] payments, GameObject[] targets)
        {
            Debug.LogError("[Game action] test action");
        }
    }
}
