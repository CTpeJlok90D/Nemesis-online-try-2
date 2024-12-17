using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Common
{
    [CreateAssetMenu(menuName = "Dictionaries/Game objects dictionary")]
    public class GameObjectsDictionary : SODictionary<string, AssetReferenceGameObject>
    {

    }
}