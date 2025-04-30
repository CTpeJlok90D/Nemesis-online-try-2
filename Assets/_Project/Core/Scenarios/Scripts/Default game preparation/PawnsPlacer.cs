using System.Linq;
using Core.CharacterInventorys;
using Core.Characters;
using Core.Maps.CharacterPawns;
using Core.Maps;
using Core.PlayerTablets;

namespace Core.Scenarios.Default
{
    public class PawnsPlacer : IChapter
    {
        private PlayerTabletList _playerTabletList;

        private PawnPlacerConfig _config;

        private RoomCell _startRoom;
        
        private KitStartConfig _kitStartConfig;

        public event IChapter.EndedListener Ended;

        public PawnsPlacer(PlayerTabletList playerTabletsList, PawnPlacerConfig pawnPlacerConfig, RoomCell startRoom, KitStartConfig kitStartConfig)
        {
            _playerTabletList = playerTabletsList;
            _startRoom = startRoom;
            _config = pawnPlacerConfig;
            _kitStartConfig = kitStartConfig;
        }

        public void Begin()
        {
            foreach (PlayerTablet tablet in _playerTabletList)
            {
                CharacterPawn characterPawn_PREFAB = _config.PawnsForCharacters[tablet.Character.Value.Id];
                CharacterPawn characterInstance = characterPawn_PREFAB.Instantiate();
                
                Character character = characterInstance.LinkedCharacter;
                InventoryItem[] startItems = _kitStartConfig.StartItems[character];
            
                characterInstance.SmallItemsInventory.AddItemsRange(startItems.Where(x => x.ItemType is ItemType.Small));
                characterInstance.BigItemsInventory.AddItemsRange(startItems.Where(x => x.ItemType is ItemType.Big));
                
                tablet.LinkPawn(characterInstance);
                _startRoom.AddContent(characterInstance.RoomContent);
            }
            Ended?.Invoke(this);
        }
    }
}
