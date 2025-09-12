using System.Collections;
using System.Linq;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerTablets;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Actions + "Destroy door")]
    public class DestroyDoor : ScriptableObject, IGameAction, INeedNoiseContainers
    {
        public const string CardID = "Destruction";
        
        public INoiseContainer[] SelectedNoiseContainers { get; set; }

        public INoiseContainer[] NoiseContainerSelectionSource
        {
            get
            {
                RoomContent content = _executor.CharacterPawn.RoomContent;
                RoomCell cell = content.Owner;
                return cell.Tunnels
                    .Where(noiseContainer 
                        => noiseContainer.NetworkObject.TryGetComponent(out Tunnel tunnel) &&
                           tunnel.DoorState is not DoorState.Broken)
                    .ToArray();
            }
        }

        public int RequiredNoiseContainerCount => 1;
        private PlayerTablet _executor;
        
        public void Initialize(PlayerTablet executor)
        {
            _executor = executor;
        }

        public IGameAction.CanExecuteCheckResult CanExecute()
        {
            if (SelectedNoiseContainers.All(selectedTunnel =>
                    NoiseContainerSelectionSource.Contains(selectedTunnel)) == false)
            {
                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = false,
                    Error = new($"Cant select this tunnels: {string.Join(", ", (IEnumerable)SelectedNoiseContainers)}")
                };
            }

            if (SelectedNoiseContainers.Length != RequiredNoiseContainerCount)
            {
                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = false,
                    Error = new($"Wrong number of tunnels: {SelectedNoiseContainers.Length}")
                };
            }

            if (_executor.ActionCardsDeck.HandLocal.Any(card => card.ID == CardID) == false)
            {
                return new()
                {
                    Result = false,
                    Error = new("No destruction card"),
                };
            }

            return new()
            {
                Result = true
            };
        }

        public void ForceExecute()
        {
            INoiseContainer container = SelectedNoiseContainers.First();
            Tunnel tunnel = container.NetworkObject.GetComponent<Tunnel>();
            tunnel.BrokeDoor();
            
            _executor.ActionCount.Value--;
            
            ActionCard card = _executor.ActionCardsDeck.HandLocal.First(x => x.ID == CardID);
            _executor.ActionCardsDeck.DiscardCard(card);
        }
    }
}