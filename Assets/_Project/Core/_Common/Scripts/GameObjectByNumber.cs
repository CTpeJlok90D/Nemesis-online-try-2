using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Common
{
    [CreateAssetMenu(menuName = "Dictionaries/Game objects by number")]
    public class GameObjectByNumber : SODictionary<int, AssetReferenceGameObject>
    {

    }
}