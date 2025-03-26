using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.DestinationCoordinats;
using Core.Engines;
using Core.EscapePods;
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

        public NetVariable<DestinationCoordinatsCard> DestinationCoordinatsCard { get; private set; }

        public NetVariable<Coordinate> Cordinates { get; private set; }

        public IReadOnlyCollection<EscapePod> EscapePods => _escapePods;

        public IReadOnlyCollection<ShipEngine> ShipEngies => _shipEngines;

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

        public void NoiseInRoom(RoomCell roomCell, NoiseDice.Result noiseDiceResult)
        {
            if (noiseDiceResult == NoiseDice.Result.Silence)
            {
                return;
            }

            if (noiseDiceResult == NoiseDice.Result.Dangerous)
            {
                foreach (INoiseContainer noiseContainer in roomCell.NoiseContainers)
                {
                    if (noiseContainer.IsNoised.Value == false)
                    {
                        noiseContainer.Noise();
                    }
                }
            }

            int tunnelIndex = (int)noiseDiceResult;
            roomCell.NoiseContainers.ElementAt(tunnelIndex).Noise();
        }
        
        public void NoiseInRoom(RoomCell roomCell)
        {
            NoiseInRoom(roomCell ,NoiseDice.Roll());
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
