using Unity.Netcode;
using UnityEngine;

namespace Core.ActionsCards
{
    [CreateAssetMenu(menuName = "Game/Action cards/Test action card")]
    public class ActionCardTestEffect : ActionCardEffect
    {
        public override void Execute(NetworkObject executor, NetworkObject[] targets)
        {
            Debug.Log("Executed");
        }
    }
}
