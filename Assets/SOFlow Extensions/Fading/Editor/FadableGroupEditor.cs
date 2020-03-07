// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SOFlow.Fading
{
    [CustomEditor(typeof(FadableGroup))]
    public class FadableGroupEditor : Editor
    {
	    /// <summary>
	    /// The FadableGroup target.
	    /// </summary>
	    private FadableGroup _target;

        private void OnEnable()
        {
            _target = (FadableGroup)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Capture Child Fadables"))
            {
                _target.CaptureChildFadables();
            }
        }
    }
}
#endif