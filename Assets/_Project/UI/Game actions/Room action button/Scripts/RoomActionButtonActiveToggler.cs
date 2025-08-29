using System.Linq;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerActions;
using Core.PlayerTablets;
using UnityEngine;


namespace UI.GameActions
{
    public class RoomActionButtonActiveToggler : MonoBehaviour
    {
        [SerializeField] private GameObject _button;
        private PlayerTablet LocalTablet => PlayerTablet.Local;
        private CharacterPawn LocalPawn => LocalTablet.CharacterPawn;
        private RoomContent LocalRoomContent => LocalPawn.RoomContent;
        private RoomCell RoomCell => LocalRoomContent.Owner;
        private IGameAction[] RoomAction => RoomCell.Type.RoomActions.Select(x => (IGameAction)x.GameAction).ToArray();
        
        private void OnEnable()
        {
            CharacterPawn.Spawned += OnCharacterSpawn;
            
            if (LocalPawn != null)
            {
                LocalRoomContent.OwnerChanged += OnOwnerChange;
            }
        }

        private void OnCharacterSpawn(CharacterPawn spawned)
        {
            if (LocalTablet.CharacterPawn == spawned)
            {
                PlayerActionExecutor.Singleton.ActionIsExecuting.Changed += OnActionIsExecutingChange;
                CharacterPawn.Spawned -= OnCharacterSpawn;
                LocalRoomContent.OwnerChanged += OnOwnerChange;
            }
        }

        private void OnDisable()
        {
            CharacterPawn.Spawned -= OnCharacterSpawn;
            if (LocalTablet != null && LocalPawn != null)
            {
                LocalRoomContent.OwnerChanged -= OnOwnerChange;
            }

            if (PlayerActionExecutor.Singleton != null)
            {
                PlayerActionExecutor.Singleton.ActionIsExecuting.Changed -= OnActionIsExecutingChange;
            }
        }

        private void OnActionIsExecutingChange(bool oldValue, bool newValue) => UpdateButtonActive();
        private void OnOwnerChange(RoomCell oldValue, RoomCell newValue) => UpdateButtonActive();
        private void UpdateButtonActive()
        {
            _button.SetActive(RoomAction != null && RoomAction.First().CanExecute());
        }
    }
}