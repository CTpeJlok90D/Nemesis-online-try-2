using Core.Aliens;
using Core.AliensBags;
using Core.Maps;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace MyNamespace
{
    public class DefaultEnemySummoner : MonoBehaviour, IEnemySummoner
    {
        [Inject] private EnemiesConfig _enemiesConfig;
        [Inject] private AliensBag _aliensBag;
        
        public async UniTask<RoomContent> SummonIn(RoomCell roomCell)
        {
            AlienToken randomToken = _aliensBag.PickRandom();
            
            if (randomToken.IsEmpty)
            {
                return null;
            }

            AssetReference enemyAssetReference = _enemiesConfig.TypeOfEnemies[randomToken];
            GameObject loadHandle = await enemyAssetReference.LoadAssetAsync<GameObject>().ToUniTask();
            Enemy enemy = loadHandle.GetComponent<Enemy>();
            enemy = enemy.Instantiate();
            roomCell.AddContent(enemy.RoomContent);

            return enemy.RoomContent;
        }
    }
}