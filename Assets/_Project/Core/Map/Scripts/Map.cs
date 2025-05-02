using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.DestinationCoordinats;
using Core.Engines;
using Core.EscapePods;
using Cysharp.Threading.Tasks;
using TNRD;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-map-96.png")]
    public class Map : NetworkBehaviour, IEnumerable<RoomCell>
    {
        [SerializeField] private Coordinate _defaultCordinats;

        [SerializeField] private RoomCell[] _roomCells;

        [SerializeField] private Tunnel[] _tunnels;

        [SerializeField] private ShipEngine[] _shipEngines;

        [SerializeField] private List<EscapePod> _escapePods;

        [SerializeField] private SerializableInterface<IEnemySummoner> _enemySummner;
        
        public NetVariable<DestinationCoordinatsCard> DestinationCoordinatsCard { get; private set; }

        public NetVariable<Coordinate> Cordinates { get; private set; }

        public IReadOnlyCollection<EscapePod> EscapePods => _escapePods;

        public IReadOnlyCollection<ShipEngine> ShipEngines => _shipEngines;

        public IReadOnlyCollection<RoomCell> RoomCells => _roomCells;

        public IReadOnlyCollection<Tunnel> Tunnels => _tunnels;

        private void Awake()
        {
            DestinationCoordinatsCard = new();
            Cordinates = new(_defaultCordinats);
        }

        public IEnumerator<RoomCell> GetEnumerator()
        {
            foreach (RoomCell cell in _roomCells)
            {
                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (RoomCell cell in _roomCells)
            {
                yield return cell;
            }
        }

        public void RemoveEscapePod(EscapePod escapePod)
        {
            RemoveEscapePod_RPC(escapePod.NetworkObject);
            escapePod.NetworkObject.Despawn(true);
        }

        [Rpc(SendTo.Everyone)]
        private void RemoveEscapePod_RPC(NetworkObjectReference reference)
        {
            _escapePods = _escapePods.Where(x => x != null).ToList();
        }

        public async UniTask NoiseInRoom(RoomCell roomCell, NoiseDice.Result noiseDiceResult)
        {
            if (noiseDiceResult == NoiseDice.Result.Silence)
            {
                return;
            }

            if (noiseDiceResult == NoiseDice.Result.Dangerous)
            {
                foreach (INoiseContainer noiseContainer in roomCell.Tunnels)
                {
                    if (noiseContainer.IsNoised.Value == false)
                    {
                        noiseContainer.Noise();
                    }
                }
                return;
            }

            INoiseContainer iNoiseContainer = roomCell.GetTunnelForNoiseRollResult(noiseDiceResult);
            
            if (iNoiseContainer.IsNoised.Value)
            {
                ClearNoiseInRoom(roomCell);
                RoomContent result = await SummonEnemyIn(roomCell);
                
                if (result == null)
                {
                    NoiseInAllTunnelsFromRoom(roomCell);
                }
                
                return;
            }
            
            iNoiseContainer.Noise();
        }

        public void CarefulNoiseInTunnel(INoiseContainer tunnel)
        {
            if (tunnel == null)
            {
                throw new ArgumentNullException();
            }

            if (tunnel.IsNoised.Value)
            {
                throw new InvalidOperationException("Can't careful noise in noised tunnel");
            }
            
            tunnel.Noise();
        }
        
        public async UniTask<NoiseDice.Result> NoiseInRoom(RoomCell roomCell)
        {
            NoiseDice.Result result = NoiseDice.Roll();
            await NoiseInRoom(roomCell, result);
            return result;
        }

        public void ClearNoiseInRoom(RoomCell roomCell)
        {
            foreach (INoiseContainer noiseContainer in roomCell.Tunnels)
            {
                noiseContainer.Clear();
            }
        }

        public void NoiseInAllTunnelsFromRoom(RoomCell roomCell)
        {
            foreach (INoiseContainer noiseContainer in roomCell.Tunnels)
            {
                noiseContainer.Noise();
            }
        }

        public async UniTask<RoomContent> SummonEnemyIn(RoomCell roomCell)
        {
            return await _enemySummner.Value.SummonIn(roomCell);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Map))]
        private class CEditor : Editor
        {
            private Map Map => target as Map;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (Map.DestinationCoordinatsCard == null)
                {
                    return;
                }
                GUI.enabled = false;
                EditorGUILayout.ObjectField(Map.DestinationCoordinatsCard.Value, typeof(DestinationCoordinatsCard), false);
                GUI.enabled = true;
            }
        }
#endif
    }
}
