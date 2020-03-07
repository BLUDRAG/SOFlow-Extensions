// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace SOFlow.Internal.SceneManagement
{
    public class SceneManagerWindow : EditorWindow
    {
        /// <summary>
        ///     The amount of scrolling that has occurred in the scene list scroll view.
        /// </summary>
        private Vector2 _sceneListScrollValue = Vector2.zero;

        /// <summary>
        ///     The list of scenes found in the project.
        /// </summary>
        private readonly List<SceneAsset> _scenes = new List<SceneAsset>();

        /// <summary>
        ///     The list of scene sets found in the project.
        /// </summary>
        private readonly List<SceneSet> _sceneSets = new List<SceneSet>();

        /// <summary>
        ///     The selected scene set.
        /// </summary>
        private int _selectedSceneSet;

        [MenuItem("SOFlow/Scene Manager")]
        public static void ShowWindow()
        {
            GetWindow<SceneManagerWindow>("SOFlow-Scene Manager");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            DrawHeaderPanel();
            EditorGUILayout.EndHorizontal();

            if(_selectedSceneSet > 0)
            {
                _sceneListScrollValue = EditorGUILayout.BeginScrollView(_sceneListScrollValue);

                foreach(SceneField scene in _sceneSets[_selectedSceneSet - 1].SetScenes)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    DrawSceneEntry(scene);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                DrawFooterPanel();
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws the header panel.
        /// </summary>
        private void DrawHeaderPanel()
        {
            List<GUIContent> sceneSetContent = new List<GUIContent>();
            sceneSetContent.Add(new GUIContent("None"));

            foreach(SceneSet set in _sceneSets)
            {
                sceneSetContent.Add(new GUIContent(set.name));
            }

            _selectedSceneSet = EditorGUILayout.Popup(_selectedSceneSet, sceneSetContent.ToArray());

            List<GUIContent> sceneContent = new List<GUIContent>
                                            {
                                                new GUIContent("Load Any Scene")
                                            };

            foreach(SceneAsset scene in _scenes)
            {
                sceneContent.Add(new GUIContent(scene.name));
            }

            int selectedScene = EditorGUILayout.Popup(0, sceneContent.ToArray());

            if(selectedScene > 0)
            {
                int loadMode = EditorUtility.DisplayDialogComplex("Load Scene", "Select load mode.", "Full",
                                                                  "Additive (Keep Current Scenes", "Cancel");

                if(loadMode == 0)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenes[selectedScene - 1]),
                                                 OpenSceneMode.Single);
                }
                else if(loadMode == 1)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenes[selectedScene - 1]),
                                                 OpenSceneMode.Additive);
                }
            }

            if(GUILayout.Button("Refresh"))
            {
                _sceneSets.Clear();
                _scenes.Clear();

                string[] assetFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
                string[] sceneFiles = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);

                foreach(string file in assetFiles)
                {
                    try
                    {
                        SceneSet set =
                            AssetDatabase.LoadAssetAtPath<SceneSet>(file.Replace($"{Application.dataPath}\\",
                                                                                 "Assets/"));

                        if(set != null)
                        {
                            _sceneSets.Add(set);
                        }
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                foreach(string file in sceneFiles)
                {
                    try
                    {
                        SceneAsset scene =
                            AssetDatabase.LoadAssetAtPath<SceneAsset>(file.Replace($"{Application.dataPath}\\",
                                                                                   "Assets/"));

                        if(scene != null)
                        {
                            _scenes.Add(scene);

                        }
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                _sceneSets.Sort((firstScene, secondScene) => string.Compare(firstScene.name, secondScene.name, StringComparison.Ordinal));
                _scenes.Sort((firstScene,    secondScene) => string.Compare(firstScene.name, secondScene.name, StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Draws the footer panel
        /// </summary>
        private void DrawFooterPanel()
        {
            if(GUILayout.Button("Load Scene Set"))
            {
                int loadMode = EditorUtility.DisplayDialogComplex("Load Scene Set", "Select load mode.", "Full",
                                                                  "Additive (Keep Current Scenes", "Cancel");

                if(loadMode == 0)
                {
                    _sceneSets[_selectedSceneSet - 1].LoadSceneSet(false);
                }
                else if(loadMode == 1)
                {
                    _sceneSets[_selectedSceneSet - 1].LoadSceneSet(true);
                }
            }

            if(GUILayout.Button("Unload Scene Set"))
            {
                _sceneSets[_selectedSceneSet - 1].UnloadSceneSet();
            }
        }

        /// <summary>
        /// Draws the scene entry
        /// </summary>
        private void DrawSceneEntry(SceneField scene)
        {
            EditorGUILayout.ObjectField(scene.SceneAsset, typeof(SceneAsset), true);

            if(GUILayout.Button("Load"))
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene.SceneAsset), OpenSceneMode.Additive);
            }

            if(GUILayout.Button("Replace"))
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene.SceneAsset), OpenSceneMode.Single);
            }

            if(GUILayout.Button("Unload"))
            {
                EditorSceneManager
                   .CloseScene(SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(scene.SceneAsset)), true);
            }
        }
    }
}
#endif