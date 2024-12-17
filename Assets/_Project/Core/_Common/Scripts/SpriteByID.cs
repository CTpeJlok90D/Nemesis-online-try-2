using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Common
{
    [CreateAssetMenu(menuName = "Dictionaries/Sprite dictionary")]
    public class SpriteByID : SODictionary<string, AssetReferenceT<Sprite>>
    {

    }
}
