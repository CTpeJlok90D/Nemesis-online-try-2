using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.Characters.Health;
using Core.Entity;
using Core.Maps;
using Core.RoomCellTokens;
using UnityEngine;

namespace Core.Scenarios.EnemiesPhase
{
    public class FireDamage : IChapter
    {
        public event IChapter.EndedListener Ended;
        
        public void Begin()
        {
            List<RoomCell> roomCells = NetEntity<RoomCell>.Instances.Where(x => x.GetContentWith<FireRoomToken>().Count() > 0).ToList();

            if (roomCells.Count == 0)
            {
                Debug.Log("Fire damage phase: No rooms with fire. skipping");
                Ended?.Invoke(this);
                return;
            }
            
            foreach (RoomCell roomCell in roomCells)
            {
                FireRoomToken fireRoomToken = roomCell.RoomContents.FirstOrDefault(x => x.GetComponent<FireRoomToken>() != null)?.GetComponent<FireRoomToken>();

                if (fireRoomToken != null)
                {
                    foreach (RoomContent roomContent in roomCell)
                    {
                        bool damaged = false;
                        
                        if (roomContent.TryGetComponent(out CharacterHealth health))
                        {
                            health.LightDamage();
                            Debug.Log($"{health} taking 1 damage from fire!");
                            damaged = true;
                        }

                        if (roomContent.TryGetComponent(out Enemy enemy))
                        {
                            enemy.Damage();
                            Debug.Log($"{enemy} taking 1 damage from fire!");
                            damaged = true;
                        }

                        if (damaged == false)
                        {
                            Debug.Log($"No content to burn in {roomCell}");
                        }
                    }
                }
            }
            
            Ended?.Invoke(this);
        }
    }
}
