using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Core.Aliens;
using Core.CharacterInventorys;
using Core.CharacterWeapons;
using Core.Maps;
using Core.PlayerTablets;
using Unity.Collections;
using UnityEngine;

namespace Core.PlayerActions.Base
{
    [CreateAssetMenu(menuName = Constants.ACTIONS_CREATE_PARH + "Attack action")]
    public class AttackAction : ScriptableObject, IGameAction, IGameActionWithPayment, IGameActionWithRoomContentSelection, IRequireInventoryItems
    {
        [SerializeField] private InventoryItem _hands;
        
        private InventoryItem _attackItem;
        
        private PlayerTablet _executor;
        public int RequaredPaymentCount => 1;
        public int RequiredItemsAmount => 1;
        public RoomContent[] RoomContentSelection { get; set; } = Array.Empty<RoomContent>();
        public InventoryItemInstance[] InventoryItemsSelection { get; set; } = Array.Empty<InventoryItemInstance>();

        public InventoryItemInstance[] InventoryItemsSource
        {
            get
            { 
                List<InventoryItemInstance> itemsWithDamage = _executor.BigItemsInventory.GetItems().Where(x => x.DataJson.ToString().Contains(WeaponDamage.DAMAGE_DATA_NAME)).ToList();
                InventoryItemInstance handsInstance = new(_hands);
                itemsWithDamage.Add(handsInstance);
                
                return itemsWithDamage.ToArray();
            }
        }
        
        public RoomContent[] RoomContentSelectionSource
        {
            get
            {
                RoomContent executorContent = _executor.CharacterPawn.RoomContent; 
                RoomCell room = executorContent.Owner;
                RoomContent[] result = room.RoomContents.Where(x => x != executorContent && x.GetComponent<Enemy>() != null).ToArray();
                return result;
            }
        }
        public int RequiredRoomContentCount => 1;
        
        public void Inititalize(PlayerTablet executor)
        {
            _executor = executor;
        }

        public IGameAction.CanExecuteCheckResult CanExecute()
        {
            if (RoomContentSelection.Length != RequiredRoomContentCount)
            {
                return new()
                {
                    Result = false,
                    Error = new InvalidOperationException("Room content selection count mismatch.")
                };
            }

            if (InventoryItemsSelection.Count() != RequiredItemsAmount)
            {
                return new()
                {
                    Result = false,
                    Error = new InvalidOperationException("Inventory item selection count mismatch.")
                };
            }
            
            InventoryItemInstance selectedWeapon = InventoryItemsSelection.First();
            string weaponDataJson = selectedWeapon.DataJson.ToString();
            SerializedDictionary<string, object> weaponData = JsonUtility.FromJson<SerializedDictionary<string, object>>(weaponDataJson);

            if (weaponData.TryGetValue(WeaponDamage.DAMAGE_DATA_NAME, out object value) == false)
            {
                return new()
                {
                    Result = false,
                    Error = new InvalidOperationException("Selected inventory item doesn't have a damage property.")
                };
            }

            RoomContent roomContent = RoomContentSelection.First();
            Enemy enemy = roomContent.GetComponent<Enemy>();

            if (enemy == null)
            {
                return new()
                {
                    Result = false,
                    Error = new InvalidOperationException("Selected room content mus be a enemy")
                };
            }
            
            if (_executor.ActionCount.Value <= 0)
            {
                IGameAction.CanExecuteCheckResult result = new()
                {
                    Result = false,
                    Error = new InvalidOperationException($"Not enough action points to execute action"),
                };

                return result;
            }

            return new()
            {
                Result = true
            };
        }

        public void Execute()
        {
            IGameAction.CanExecuteCheckResult result = CanExecute();
            if (result == false)
            {
                throw result.Error;
            }

            ForceExecute();
        }

        public void ForceExecute()
        {
            RoomContent roomContent = RoomContentSelection.First();
            Enemy enemy = roomContent.GetComponent<Enemy>();

            InventoryItemInstance selectedWeapon = InventoryItemsSelection.First();
            string weaponDataJson = selectedWeapon.DataJson.ToString();
            SerializedDictionary<string, string> weaponData = JsonUtility.FromJson<SerializedDictionary<string, string>>(weaponDataJson);

            _executor.ActionCount.Value--;
            AttackDice.Result rollResult = AttackDice.Roll();

            int damage = 0;
            string value = "";

            if (weaponData.TryGetValue(WeaponDamage.DAMAGE_DATA_NAME, out value))
            {
                damage = Convert.ToInt32(value);
            }

            if (weaponData.TryGetValue(WeaponDamage.CONST_DAMAGE_DATA_NAME, out value))
            {
                damage += Convert.ToInt32(value);
            }

            if (weaponData.TryGetValue(WeaponDamage.ADDITIONAL_DAMAGE_DATA_NAME, out value))
            {
                SerializedDictionary<AttackDice.Result, int> additionalDamage = JsonUtility.FromJson<SerializedDictionary<AttackDice.Result, int>>(value);

                if (additionalDamage.ContainsKey(rollResult))
                {
                    damage += additionalDamage[rollResult];
                }
            }
            
            if (enemy.AttacksToHit.Contains(rollResult))
            {
                enemy.Damage(damage);
            }
        }
    }
}
