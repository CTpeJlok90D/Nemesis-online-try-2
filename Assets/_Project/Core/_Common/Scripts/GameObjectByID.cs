using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Common
{
    [CreateAssetMenu(menuName = "Dictionaries/Game objects by id dictionary")]
    public class GameObjectByID : SODictionary<string, AssetReferenceGameObject>
    {

    }
}