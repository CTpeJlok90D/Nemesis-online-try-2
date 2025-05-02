using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Core.CharacterInventories;
using Core.Characters;
using Core.Maps.CharacterPawns;
using Core.Maps;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            _ = PlacePawns();
        }

        private async UniTask PlacePawns()
        {
            foreach (PlayerTablet tablet in _playerTabletList)
            {
                CharacterPawn characterPawn_PREFAB = _config.PawnsForCharacters[tablet.Character.Value.Id];
                CharacterPawn characterInstance = characterPawn_PREFAB.Instantiate();
                
                Character character = characterInstance.LinkedCharacter;
                string[] startItemsIds = _kitStartConfig.StartItems[character];
                List<InventoryItem> loadedItems = new();
                foreach (string itemID in startItemsIds)
                {
                    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(itemID);
                    await handle.ToUniTask();
                    loadedItems.Add(handle.Result.GetComponent<InventoryItem>());
                }
                
                characterInstance.SmallItemsInventory.AddItemsRange(loadedItems.Where(x => x.ItemType is ItemType.Small));
                characterInstance.BigItemsInventory.AddItemsRange(loadedItems.Where(x => x.ItemType is ItemType.Big));
                
                tablet.LinkPawn(characterInstance);
                _startRoom.AddContent(characterInstance.RoomContent);
            }
            Ended?.Invoke(this);
        }
    }
}
