using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Netcode.Custom
{
    [CreateAssetMenu(menuName = "Netcode/Network prefabs auto list")]
    public class NetworkPrefabsAutoList : NetworkPrefabsList
    {
        [SerializeField] private string _searchFilter = "t:prefab l:";

#if UNITY_EDITOR

        private bool isValidated; 

        private void OnValidate()
        {
            if (isValidated)
            {
                return;
            }
            isValidated = true;

            Validate();
        }
        
        private void Validate()
        {
            if (EditorApplication.isUpdating)
            {
                return;
            }

            ForceValidate();
        }

        [ContextMenu(nameof(Validate))]
        private void ForceValidate()
        {
            SerializedObject serializedSelf = new(this);
            SerializedProperty serializedList = serializedSelf.FindProperty("List");
            serializedList.arraySize = 0;

            string[] assetGUIDs = AssetDatabase.FindAssets(_searchFilter);
            string[] assetsPaths = assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

            foreach (string assetPath in assetsPaths)
            {
                GameObject loadedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                NetworkPrefab netPrefab = new()
                {
                    Prefab = loadedAsset
                };
                serializedList.arraySize++;
                serializedList.GetArrayElementAtIndex(serializedList.arraySize - 1).FindPropertyRelative("Prefab").objectReferenceValue = loadedAsset;
            }

            serializedSelf.ApplyModifiedProperties();
        }
#endif
    }
}
