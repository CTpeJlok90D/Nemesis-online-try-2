using Core.Maps.CharacterPawns;
using Core.Maps;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.Default
{
    public class PawnsPlacer : IChapter
    {
        private PlayerTabletList _playerTabletList;

        private PawnPlacerConfig _config;

        private RoomCell _startRoom;

        public event IChapter.EndedListener Ended;

        public PawnsPlacer(PlayerTabletList playerTabletsList, PawnPlacerConfig pawnPlacerConfig, RoomCell startRoom)
        {
            _playerTabletList = playerTabletsList;
            _startRoom = startRoom;
            _config = pawnPlacerConfig;
        }

        public void Begin()
        {
            foreach (PlayerTablet tablet in _playerTabletList)
            {
                CharacterPawn characterPawn_PREFAB = _config.PawnsForCharacters[tablet.Character.Value.Id];
                
                characterPawn_PREFAB.gameObject.SetActive(false);
                CharacterPawn characterInstance = Object.Instantiate(characterPawn_PREFAB);
                characterPawn_PREFAB.gameObject.SetActive(true);
                
                characterInstance.gameObject.SetActive(true);
                characterInstance.NetworkObject.Spawn();
                
                tablet.LinkPawn(characterInstance);
                _startRoom.AddContent(characterInstance.RoomContent);
            }
            Ended?.Invoke(this);
        }
    }
}
