// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SOFlow.Fading
{
    [CustomEditor(typeof(GenericFader))]
    public class GenericFaderEditor : Editor
    {
        /// <summary>
        /// The GenericFader target.
        /// </summary>
        private GenericFader _target;

        private void OnEnable()
        {
            _target = (GenericFader)target;
        }

        public override void OnInspectorGUI()
        {
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeTargets"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UnfadedColour"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadedColour"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeCurve"));

            if(!_target.OnlyFadeIn)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UnfadeCurve"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnlyFadeIn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeTime"));

            if(!_target.OnlyFadeIn)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UnfadeTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("WaitBetweenFades"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFadeStart"));

            if(!_target.OnlyFadeIn)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFadeWait"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFadeComplete"));

            if(GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif