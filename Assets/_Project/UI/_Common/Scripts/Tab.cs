using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.CommonScripts
{
    [Icon("Assets/_Project/UI/_Ñommon scripts/Editor/Icons/icons8-tab-96.png")]
    public class Tab : MonoBehaviour
    {
        [SerializeField] private UnityEvent _enabled;

        public event UnityAction Enabled
        {
            add => _enabled.AddListener(value);
            remove => _enabled.RemoveListener(value);
        }

        public void Enable()
        {
            DisableAllOtherTabs();
            gameObject.SetActive(true);
        }

        private void DisableAllOtherTabs()
        {
            foreach (Transform child in transform.parent)
            {
                child.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Tab))]
        public class CEditor : Editor
        {
            public Tab _target => target as Tab;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Enable"))
                {
                    _target.Enable();
                    EditorUtility.SetDirty(this);
                    foreach (Transform child in _target.transform.parent)
                    {
                        EditorUtility.SetDirty(child);
                    }
                }
            }
        }
#endif
    }
}