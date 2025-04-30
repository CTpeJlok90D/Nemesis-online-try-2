using Core.Characters.Health;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class HealthCheats : MonoBehaviour
{
    [Inject] private PlayerTabletList _playerTabletList;
    
    private void Awake()
    {
        DebugLogConsole.AddCommand<string>("Kill", "Kill player", KillPlayer);
        DebugLogConsole.AddCommand<string, int>("Damage.Light", "Add light damage to player", LightDamagePlayer);
        DebugLogConsole.AddCommand<string, string>("Damage.Heavy", "Add heavy damage to player", (nickname, damageID) => _ = HeavyDamagePlayer(nickname, damageID));
    }
    
    private void KillPlayer(string playerNickname)
    {
        Target target = new Target(playerNickname, _playerTabletList);

        foreach (PlayerTablet playerTablet in target)
        {
            CharacterHealth health = playerTablet.Health;
            health.ForceKill();
        }

        Debug.Log($"{target} was killed");
    }

    private void LightDamagePlayer(string playerNickname, int damage)
    {
        Target target = new Target(playerNickname, _playerTabletList);

        foreach (PlayerTablet playerTablet in target)
        {
            CharacterHealth health = playerTablet.Health;
            health.LightDamage(damage);
        }
        
        Debug.Log($"{target} was suffered light damage of {damage}");
    }
    
    private async UniTask HeavyDamagePlayer(string playerNickname, string damageID)
    {
        Target target = new Target(playerNickname, _playerTabletList);

        AsyncOperationHandle<HeavyDamage> handle = Addressables.LoadAssetAsync<HeavyDamage>(damageID);
        await handle.ToUniTask();
        HeavyDamage damage = handle.Result;
        
        foreach (PlayerTablet playerTablet in target)
        {
            CharacterHealth health = playerTablet.Health;
            health.HeavyDamage(damage);
        }
        
        Debug.Log($"{target} was suffered heavy damage of {damage}");
    }
}
