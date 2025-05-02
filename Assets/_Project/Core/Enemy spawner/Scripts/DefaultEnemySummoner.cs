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
            return await SummonIn(roomCell, randomToken);
        }

        public async UniTask<RoomContent> SummonIn(RoomCell roomCell, AlienToken alienToken)
        {
            Debug.Log($"Summoning enemy: {alienToken}. Room: {roomCell}");
            
            if (alienToken.IsEmpty)
            {
                return null;
            }

            AssetReference enemyAssetReference = _enemiesConfig.TypeOfEnemies[alienToken];
            Enemy enemy;

            if (enemyAssetReference.OperationHandle.IsValid())
            {
                if (enemyAssetReference.OperationHandle.IsDone)
                {
                    await enemyAssetReference.OperationHandle.ToUniTask();
                }
                
                GameObject loadResult = (GameObject)enemyAssetReference.OperationHandle.Result;
                enemy = loadResult.GetComponent<Enemy>();
            }
            else
            {
                GameObject loadResult = await enemyAssetReference.LoadAssetAsync<GameObject>().ToUniTask();
                enemy = loadResult.GetComponent<Enemy>();
            }
            
            enemy = enemy.Instantiate(alienToken);
            roomCell.AddContent(enemy.RoomContent);

            return enemy.RoomContent;
        }
    }
}