using UnityEngine;

/// <summary>
/// Code updated by Gaskellgames
/// </summary>

namespace Gaskellgames.FolderSystem
{
    [ExecuteInEditMode]
    [Icon("Assets/Gaskellgames/Folder System/Editor/icons8-folder-96.png")]
    public class HierarchyFolders : GGMonoBehaviour
    {
        #region Variables

        public enum TextAlignment { Left, Center }

        [SerializeField]
        public bool customText;

        [SerializeField]
        public bool customIcon;

        [SerializeField]
        public bool customHighlight;

        [SerializeField]
        public Color32 textColor = InspectorUtility.textNormalColor;

        [SerializeField]
        public Color32 iconColor = InspectorUtility.textNormalColor;

        [SerializeField]
        public Color32 highlightColor = InspectorUtility.cyanColor;

        [SerializeField]
        public FontStyle textStyle = FontStyle.BoldAndItalic;

        [SerializeField]
        public TextAlignment textAlignment = TextAlignment.Left;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor Loop

        private void OnValidate()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        #endregion

    } // class end
}