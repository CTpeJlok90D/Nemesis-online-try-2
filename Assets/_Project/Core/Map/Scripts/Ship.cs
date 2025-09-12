using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.DestinationCoordinats;
using Core.Engines;
using Core.Entities;
using Core.EscapePods;
using Core.PlayerActions.Base;
using Core.RoomCellTokens;
using Core.TimeTracks;
using Cysharp.Threading.Tasks;
using TNRD;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;

namespace Core.Maps
{
    [Icon(EditorSOIcons.SpaceShip)]
    public class Ship : NetEntity<Ship>, IEnumerable<RoomCell>
    {
        [SerializeField] private Coordinate _defaultCoordinates;
        [SerializeField] private RoomCell[] _roomCells;
        [SerializeField] private Tunnel[] _tunnels;
        [SerializeField] private ShipEngine[] _shipEngines;
        [SerializeField] private List<EscapePod> _escapePods;
        [SerializeField] private SerializableInterface<IEnemySummoner> _enemySummner;
        [SerializeField] private TimeTrack _selfDestructionTimeTrack;
        
        public NetVariable<DestinationCoordinatesCard> DestinationCoordinatesCard { get; private set; }
        public NetVariable<Coordinate> Coordinate { get; private set; }
        public Destination Destination => DestinationCoordinatesCard.Value.CoordinatesForDestinations[Coordinate.Value];
        public NetVariable<int> MaxFireTokenCount { get; private set; }
        public NetVariable<int> MaxMalfunctionTokenCount { get; private set; }
        public IReadOnlyCollection<EscapePod> EscapePods => _escapePods;
        public IReadOnlyCollection<ShipEngine> ShipEngines => _shipEngines;
        public IReadOnlyCollection<RoomCell> RoomCells => _roomCells;
        public IReadOnlyCollection<Tunnel> Tunnels => _tunnels;

        public bool IsDestroyed =>
            _roomCells.Count(x => x.GetContentWith<FireRoomToken>() != null) > MaxFireTokenCount.Value ||
            _roomCells.Count(x => x.GetContentWith<MalfunctionRoomToken>() != null) > MaxMalfunctionTokenCount.Value ||
            _selfDestructionTimeTrack.Current.Value == 0;

        private void Awake()
        {
            DestinationCoordinatesCard = new();
            Coordinate = new(_defaultCoordinates);
            MaxFireTokenCount = new();
            MaxMalfunctionTokenCount = new();
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
            escapePod.NetworkObject.Despawn();
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
        [CustomEditor(typeof(Ship))]
        private class CEditor : Editor
        {
            private Ship Ship => target as Ship;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (Ship.DestinationCoordinatesCard == null)
                {
                    return;
                }
                GUI.enabled = false;
                EditorGUILayout.ObjectField(Ship.DestinationCoordinatesCard.Value, typeof(DestinationCoordinatesCard), false);
                GUI.enabled = true;
            }
        }
#endif
    }
}
