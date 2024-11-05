using System;
using System.Threading.Tasks;
using Core.Players;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.PlayerTablets
{
    public class PlayerTablet : NetworkBehaviour
    {
        public NetBehaviourReference<Player> PlayerReference { get; private set; }

        public bool CanBookIt => PlayerReference.Reference == null;

        private bool _haveResult;
        private ToBookResult _result;

        private void Awake()
        {
            PlayerReference = new();
        }
        
        public async Task<ToBookResult> ToBookItFor(Player player)
        {
            try
            {
                ToBookItFor_RPC(player.NetworkObject);
                while (_haveResult == false)
                {
                    await Awaitable.NextFrameAsync();
                }
                _haveResult = false;

                return _result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return ToBookResult.Failure;
            }

        }


        [Rpc(SendTo.Server)]
        private void ToBookItFor_RPC(NetworkObjectReference playerReference)
        {
            if (playerReference.TryGet(out NetworkObject playerObject) == false)
            {
                return;
            }

            Player player = playerObject.GetComponent<Player>();
            if (CanBookIt)
            {
                PlayerReference.Reference = player;
                SendResult_RPC(ToBookResult.Success, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Persistent));

                return;
            }
            SendResult_RPC(ToBookResult.Failure, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Persistent));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendResult_RPC(ToBookResult result, RpcParams rpcParams = default)
        {
            _result = result;
            _haveResult = true;
        }
    }

    public enum ToBookResult { Success, Failure }
}
