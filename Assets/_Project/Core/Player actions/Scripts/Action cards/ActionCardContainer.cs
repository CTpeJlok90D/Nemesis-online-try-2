using System;
using UnityEditor;
using UnityEngine;
using Zenject;


namespace Core.ActionsCards
{
    public class ActionCardContainer : MonoBehaviour
    {
        public ActionCard ActionCard { get; private set; }

        public ActionCardContainer Instantiate(ActionCard actionCard, DiContainer diContainer, Transform parent = null)
        {
            gameObject.SetActive(false);
            ActionCardContainer actionCardContainer = Instantiate(this, parent);
            diContainer.InjectGameObject(actionCardContainer.gameObject);
            gameObject.SetActive(true);

            actionCardContainer.ActionCard = actionCard;
            actionCardContainer.gameObject.SetActive(true);
            return actionCardContainer;
       }

#if UNITY_EDITOR
        [CustomEditor(typeof(ActionCardContainer))]
        private class CEditor : Editor
        {
            public ActionCardContainer ActionCardContainer => target as ActionCardContainer;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(ActionCardContainer.ActionCard, typeof(ActionCard), false);
                GUI.enabled = true;
            }
        }
#endif
    }
}