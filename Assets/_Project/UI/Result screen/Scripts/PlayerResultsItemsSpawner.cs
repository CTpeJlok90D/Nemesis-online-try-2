using System.Collections.Generic;
using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class PlayerResultsItemsSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerTabletContainer _playerResultItem_PREFAB;
        [SerializeField] private Transform _playersTransform;

        private readonly List<PlayerTabletContainer> _instances = new();
        
        private void OnEnable()
        {
            ShowResults();
        }

        private void ShowResults()
        {
            ClearInstances();
            SpawnInstances();
        }

        private void SpawnInstances()
        {
            foreach (PlayerTablet playerTablet in PlayerTablet.Instances)
            {
                PlayerTabletContainer instance = _playerResultItem_PREFAB.Instantiate(playerTablet, parent:_playersTransform);
                _instances.Add(instance);
            }
        }

        private void ClearInstances()
        {
            foreach (PlayerTabletContainer playerTabletContainer in _instances)
            {
                Destroy(playerTabletContainer);
            }
            
            _instances.Clear();
        }
    }
}