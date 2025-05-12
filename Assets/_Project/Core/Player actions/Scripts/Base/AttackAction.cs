using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.CharacterInventories;
using Core.CharacterWeapons;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using UnityEditor.Android;
using UnityEngine;

namespace Core.PlayerActions.Base
{
    [CreateAssetMenu(menuName = Constants.ACTIONS_CREATE_PARH + "Attack action")]
    public class AttackAction : ScriptableObject, IGameAction, INeedPayment, INeedRoomContents, INeedInventoryItems
    {
        private InventoryItem _attackItem;
        
        private PlayerTablet _executor;
        private InventoryItem _handsInstance;
        public int RequaredPaymentCount => 1;
        public int RequiredItemsAmount => 1;
        public RoomContent[] RoomContentSelection { get; set; } = Array.Empty<RoomContent>();
        public InventoryItem[] InventoryItemsSelection { get; set; } = Array.Empty<InventoryItem>();

        public InventoryItem[] InventoryItemsSource
        {
            get
            {
                List<InventoryItem> itemsWithDamage = _executor.BigItemsInventory.GetItems()
                    .Where(x => 
                        x.TryGetComponent(out WeaponDamage weaponDamage) 
                        && 
                            (
                                x.TryGetComponent(out MagazineSize magazineSize) == false 
                                || 
                                magazineSize.AmmoCount.Value > 0)
                            )
                    .ToList();
                
                itemsWithDamage.Add(CharacterPawn.Hands);
                
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
            
            InventoryItem selectedWeapon = InventoryItemsSelection.First();
            WeaponDamage weaponDamage = selectedWeapon.GetComponent<WeaponDamage>();
            MagazineSize magazineSize = selectedWeapon.GetComponent<MagazineSize>();

            if (weaponDamage == null || magazineSize != null && magazineSize.AmmoCount.Value == 0)
            {
                return new()
                {
                    Result = false,
                    Error = new InvalidOperationException("Selected weapon is invalid")
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

            InventoryItem selectedWeapon = InventoryItemsSelection.First();

            WeaponDamage weaponDamage = selectedWeapon.GetComponent<WeaponDamage>();
            
            AttackDice.Result rollResult = AttackDice.Roll();
            int damage = weaponDamage.GetDamageFor(rollResult, enemy);

            if (selectedWeapon.TryGetComponent(out MagazineSize magazineSize))
            {
                magazineSize.AmmoCount.Value--;
            }
            
            enemy.Damage(damage);
            if (enemy.AttacksToHit.Contains(rollResult))
            {
                Debug.Log($"Damage: {damage} to {enemy}. Roll Result: {rollResult}");
            }
            else
            {
                Debug.Log($"Miss! Roll result: {rollResult}. Required to hit: {string.Join(", ", enemy.AttacksToHit)}");
            }

            foreach (IAttackPostProcessor postProcessor in selectedWeapon.GetComponents<IAttackPostProcessor>())
            {
                postProcessor.PostProcessAttack(_executor, enemy, rollResult);
            }
        }
    }
}
