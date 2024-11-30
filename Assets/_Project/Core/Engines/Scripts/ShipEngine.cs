using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;

namespace Core.Engines
{
    [Icon("Assets/_Project/Core/Engines/Editor/icons8-engine-96.png")]
    public class ShipEngine : NetworkBehaviour
    {
        public NetVariable<bool> IsWorking { get; private set; }

        private void Awake()
        {
            IsWorking = new();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ShipEngine))]
        private class CEditor : Editor
        {
            private ShipEngine ShipEngine => target as ShipEngine;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (ShipEngine.IsWorking != null)
                {
                    GUI.enabled = false;
                    GUILayout.Toggle(ShipEngine.IsWorking.Value, "Is working");
                    GUI.enabled = true;
                }
            }
        }
#endif
    }
}
