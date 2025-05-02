using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.AliensBags;
using Core.AliensTablets;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using MyNamespace;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.EnemiesPhase
{
    public class HiveDevelopment : IChapter
    {
        [Inject] private AliensBag _aliensBag;
        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private AliensTablet _aliensTablet;
        [Inject] private Map _map;

        private RoomCell _hiveRoom;
        private DefaultEnemySummoner _defaultEnemySummoner;
        private Config _config;

        public HiveDevelopment(Config config, RoomCell hiveRoom, DefaultEnemySummoner defaultEnemySummoner)
        {
            _config = config;
            _hiveRoom = hiveRoom;
            _defaultEnemySummoner = defaultEnemySummoner;
        }
        
        public void Begin()
        {
            AlienToken token = _aliensBag.PickRandom();
            switch (token.AlienType)
            {
                case "Adult intruder":
                    Debug.Log($"Hive development: adult intruder. All players throwing a noise dice");
                    _aliensBag.Add(token);
                    _ = AllNoise();
                    break;
                
                case "Breeder":
                    Debug.Log($"Hive development: breeder. All players throwing a noise dice");
                    _aliensBag.Add(token);
                    _ = AllNoise();
                    break;
                
                case "Creeper":
                    Debug.Log($"Hive development: Creeper. Breeder was added to pull");
                    _aliensBag.Add(_config.Breeder);
                    break;
                
                case "Empty":
                    Debug.Log($"Hive development: Empty. Adult intruder was added to pull");
                    _aliensBag.Add(_config.AdultIntruderToken);
                    break;
                
                case "Larvae":
                    Debug.Log($"Hive development: Larvae. Adult intruder was added to pull");
                    _aliensBag.Add(_config.AdultIntruderToken);
                    break;
                
                case "Queen":
                    Debug.Log($"Hive development: Queen.");
                    _ = ComingOfTheQueen();
                    break;
            }
            Ended?.Invoke(this);
        }

        public event IChapter.EndedListener Ended;

        public async UniTask AllNoise()
        {
            foreach (PlayerTablet playerTablet in _playerTabletList.OrderBy(x => x.OrderNumber.Value))
            {
                RoomCell roomCell = playerTablet.CharacterPawn.RoomContent.Owner;
                NoiseDice.Result rollResult = await _map.NoiseInRoom(roomCell);
                
                Debug.Log($"Noise dice result for {playerTablet.Nickname} : {rollResult}");
            }
        }

        public async UniTask ComingOfTheQueen()
        {
            CharacterPawn[] characters = _hiveRoom.GetContentWith<CharacterPawn>().ToArray();

            if (characters.Any())
            {
                Debug.Log($"{string.Join(", ", characters.ToList())} in the hive! The are summoned the queen!");
                
                _aliensBag.Remove(_config.Queen);
                await _defaultEnemySummoner.SummonIn(_hiveRoom, _config.Queen);
                return;
            }

            Debug.Log($"Egg count was increased");
            _aliensTablet.EggCount.Value++;
        }
        
        [Serializable]
        public struct Config
        {
            public AlienToken AdultIntruderToken;
            public AlienToken Breeder;
            public AlienToken Creeper;
            public AlienToken Empty;
            public AlienToken Larvae;
            public AlienToken Queen;
        }
    }
}
