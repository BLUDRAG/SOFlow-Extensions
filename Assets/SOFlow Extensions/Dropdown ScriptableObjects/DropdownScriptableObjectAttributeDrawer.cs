// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SOFlow.ScriptableObjects
{
	[CustomPropertyDrawer(typeof(DropdownScriptableObject), true)]
	public class DropdownScriptableObjectAttributeDrawer : PropertyDrawer
	{
		/// <summary>
		/// The set of available dropdowns.
		/// </summary>
		public static readonly Dictionary<Type, List<ScriptableObject>> AvailableDropdowns = new Dictionary<Type, List<ScriptableObject>>();
		
		/// <summary>
		/// The none dropdown option.
		/// </summary>
		private readonly GUIContent _noneOption = new GUIContent("None");

		/// <summary>
		/// The create new dropdown option.
		/// </summary>
		private readonly GUIContent _createNewOption = new GUIContent("--- Create New ---");
		
		/// <summary>
		/// The cached object type.
		/// </summary>
		private Type _objectType;

		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(_objectType == null)
			{
				_objectType = GetInstanceType(property.type);
			}

			if(_objectType == null)
			{
				EditorGUI.PropertyField(position, property, label);
			}
			else
			{
				List<ScriptableObject> dropdowns;

				if(!AvailableDropdowns.TryGetValue(_objectType, out dropdowns))
				{
					AvailableDropdowns.Add(_objectType, new List<ScriptableObject>());
				}
				else
				{
					ValidateDropdownEntries();
					SortDropdownEntries();
					
					int selection = 0;
					int optionLength = dropdowns.Count + 2;
					
					GUIContent[] options = new GUIContent[optionLength];
					options[0] = _noneOption;
					options[1] = _createNewOption;

					for(int i = 2; i < optionLength; i++)
					{
						if(dropdowns[i - 2] == property.objectReferenceValue)
						{
							selection = i;
						}

						options[i] = new GUIContent($"{dropdowns[i - 2].name} ({dropdowns[i - 2].GetType().Name})", AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(dropdowns[i - 2])));
					}

					UnityEngine.Object objectReference = property.objectReferenceValue;

					if(objectReference != null)
					{
						position.width -= 24f;
					}

					EditorGUI.BeginChangeCheck();
					
					selection = EditorGUI.Popup(position, label, selection, options);

					if(EditorGUI.EndChangeCheck())
					{
						if(selection == 0)
						{
							property.objectReferenceValue = null;
						}
						else if(selection == 1)
						{
							Type objectType = GetInstanceType(property.type);
							
							string objectPath = EditorUtility.SaveFilePanelInProject($"Create New {objectType?.Name}",
							                                                         $"New {objectType?.Name}",
							                                                         "asset",
							                                                         $"Create new {objectType?.Name} dropdown item.");

							if(!string.IsNullOrEmpty(objectPath) && objectType != null)
							{
								ScriptableObject newObject = ScriptableObject.CreateInstance(objectType);
								
								AssetDatabase.CreateAsset(newObject, objectPath);
								AssetDatabase.Refresh();
								
								SortDropdownEntries();
								property.objectReferenceValue = newObject;
							}
						}
						else
						{
							property.objectReferenceValue = dropdowns[selection - 2];
						}
						
						property.serializedObject.ApplyModifiedProperties();
					}

					if(objectReference != null)
					{
						position.x     += position.width + 2f;
						position.width =  22f;

						if(GUI.Button(position, "→"))
						{
							Selection.activeObject = objectReference;
							EditorGUIUtility.PingObject(objectReference);
						}
					}
				}
			}
		}

		/// <summary>
		/// Removes any non-existing entries from the list of available dropdown items.
		/// </summary>
		private static void ValidateDropdownEntries()
		{
			foreach(KeyValuePair<Type, List<ScriptableObject>> dropdownData in AvailableDropdowns)
			{
				ValidateListEntries(dropdownData.Value, dropdown => dropdown);
			}
		}

		/// <summary>
		/// Sorts the dropdown entries.
		/// </summary>
		private static void SortDropdownEntries()
		{
			foreach(KeyValuePair<Type, List<ScriptableObject>> dropdownData in AvailableDropdowns)
			{
				dropdownData.Value.Sort((first, second) => string.Compare(first.name, second.name, StringComparison.Ordinal));
			}
		}
		
		// ##### Extensions #####

		/// <summary>
		/// Returns the Type instance of the given class name.
		/// </summary>
		/// <param name="strFullyQualifiedName"></param>
		/// <returns></returns>
		private static Type GetInstanceType(string strFullyQualifiedName)
		{
			string sanitizedName = strFullyQualifiedName.Replace("PPtr<$", "").Replace(">", "");
			Type   type          = Type.GetType(sanitizedName);

			if(type != null) return type;

			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				Module[] modules = asm.GetModules();

				foreach(Module module in modules)
				{
					foreach(Type _type in module.GetTypes())
					{
						if(_type.Name == sanitizedName)
							return _type;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Removes any null entries from the list.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="entryExistenceCheck"></param>
		/// <typeparam name="T"></typeparam>
		private static void ValidateListEntries<T>(List<T> list, Func<T, bool> entryExistenceCheck = null)
		{
			List<int> entriesToRemove = new List<int>();

			for(int i = 0; i < list.Count; i++)
				if(list[i] == null || entryExistenceCheck != null && !entryExistenceCheck.Invoke(list[i]))
					entriesToRemove.Add(i);

			for(int i = entriesToRemove.Count - 1; i >= 0; i--) list.RemoveAt(entriesToRemove[i]);
		}
	}
}
#endif