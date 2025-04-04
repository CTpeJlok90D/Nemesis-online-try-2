using System;
using Core.Maps;
using Unity.Netcode;
using UnityEngine;

namespace Core.Aliens
{
    [Icon("Assets/_Project/Core/Alien/Editor/monster.png")]
    [RequireComponent(typeof(RoomContent))]
    public class Enemy : NetworkBehaviour
    {
        public RoomContent RoomContent { get; private set; }

        public Enemy Instantiate()
        {
            gameObject.SetActive(false);
            Enemy enemy = Instantiate(this);
            gameObject.SetActive(true);
            
            enemy.gameObject.SetActive(true);
            enemy.NetworkObject.Spawn();
            return enemy;
        }

        private void Awake()
        {
            RoomContent = GetComponent<RoomContent>();
        }
    }
}
