using Unity.Netcode;
using UnityEngine;

namespace Core.ActionsCards
{
    [Icon("Assets/_Project/Core/Actions cards/Editor/icons8-empty-cards-96.png")]
    public abstract class ActionCardEffect : ScriptableObject
    {
        public abstract void Execute(NetworkObject executor, NetworkObject[] targets);
    }
}
