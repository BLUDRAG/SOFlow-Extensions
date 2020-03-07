// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using UnityEditor;

namespace SOFlow.Fading
{
	[CustomEditor(typeof(MaterialFadable))]
	public class MaterialFadableEditor : Editor
	{
		/// <summary>
		/// The MaterialFadable target.
		/// </summary>
		private MaterialFadable _target;

		private void OnEnable()
		{
			_target = (MaterialFadable)target;
		}

		public override void OnInspectorGUI()
		{
			using(new EditorGUI.DisabledScope(true))
			{
			   EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.AlphaOnly)));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.InvertAlpha)));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.InvertPercentage)));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.UseRenderer)));

			if(_target.UseRenderer)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.TargetRenderer)));
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.TargetMaterial)));
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.OverrideColourProperty)));

			if(_target.OverrideColourProperty)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_target.ColourProperty)));
			}
		}
	}
}
#endif