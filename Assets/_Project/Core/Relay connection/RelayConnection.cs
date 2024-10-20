using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Core.Conncetion
{
    public class RelayConnection
    {
        public event Action<string> Started;
        public event Action Joined;
        public string JoinCode { get; private set; }

        public async Task CreateRelay()
        {
            if (!AuthenticationService.Instance.IsAuthorized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
            }
            
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                    );
                
                NetworkManager.Singleton.StartHost();
                
                Started?.Invoke(JoinCode);
                Joined?.Invoke();
            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async Task JoinRelay(string joinCode)
        {
            if (!AuthenticationService.Instance.IsAuthorized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
            }

            try
            {
                JoinCode = joinCode;
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                    );

                NetworkManager.Singleton.StartClient();
                
                Joined?.Invoke();
            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }
}