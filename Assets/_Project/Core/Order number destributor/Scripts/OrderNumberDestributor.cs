using System.Collections.Generic;
using System.Linq;
using Core.PlayerTablets;
using Core.Scenarios;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.OrderNumberDestributors
{
    public class OrderNumberDestributor : NetworkBehaviour, IChapter
    {
        public delegate void OrderNumbersWereDistributedListener();

        public event OrderNumbersWereDistributedListener OrderNumbersWereDistributed;
        public event IChapter.EndedListener Ended;

        private void DistributeOrderNumbers()
        {
            List<int> numbers = new();
            for (int i = 1; i <= PlayerTablet.Instances.Count; i++)
            {
                numbers.Add(i);
            }

            foreach (PlayerTablet playerTablet in PlayerTablet.Instances)
            {
                int number = Random.Range(0, PlayerTablet.Instances.Count);
                playerTablet.OrderNumber.Value = number;
                numbers.Remove(number);
            }

            DistributedCallback_RPC();
        }

        public void Begin()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                DistributeOrderNumbers();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void DistributedCallback_RPC()
        {
            OrderNumbersWereDistributed?.Invoke();
            Ended?.Invoke(this);
        }
    }
}
