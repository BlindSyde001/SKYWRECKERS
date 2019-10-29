using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using DiasGames.ThirdPersonSystem.Cameras;

namespace DiasGames.ThirdPersonSystem
{
    public class SetupScene : EditorWindow
    {
        protected GUISkin windowSkin;
                
        string setupLabel = string.Empty;
        MessageType messageType = MessageType.Info;


        [MenuItem("Tools/Third Person System/Setup Scene")]
        public static void ShowWindow()
        {
            GetWindow<SetupScene>(true, "Setup Scene");
        }

        private void OnEnable()
        {
            windowSkin = Resources.Load("WindowSkin") as GUISkin;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(windowSkin.box);
            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Click on the settings you want to add to your scene", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Setup the Camera", windowSkin.button))
            {
                if (SetCamera())
                {
                    setupLabel = "Camera Rig was added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Camera Rig was not added because your scene already has a Camera Rig.";
                    messageType = MessageType.Warning;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Create the Game Controller", windowSkin.button))
            {
                if (CreateGameController())
                {
                    setupLabel = "Game Controller was created and added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Game Controller was not created because your scene already has a Game Controller.";
                    messageType = MessageType.Warning;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Canvas UI", windowSkin.button))
            {
                if (SetUI())
                {
                    setupLabel = "Canvas UI was added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Canvas UI was not added because your scene already has a Canvas UI.";
                    messageType = MessageType.Warning;
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (setupLabel != string.Empty)
                EditorGUILayout.HelpBox(setupLabel, messageType);

            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", windowSkin.button))
                Close();
            ///

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private bool SetCamera()
        {
            CameraController cam = FindObjectOfType<CameraController>();

            if (cam == null)
            {
                cam = AssetDatabase.LoadAssetAtPath<CameraController>("Assets/Dias Games/Third Person System/Prefabs/CameraRig.prefab");
                Transform character = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                CameraController game;
                if (character != null)
                    game = Instantiate(cam, character.position, character.rotation);
                else
                    game = Instantiate(cam);

                game.gameObject.name = game.gameObject.name.Replace("(Clone)", string.Empty);

                return true;
            }

            return false;
        }

        private bool CreateGameController()
        {
            GameObject controller = GameObject.FindGameObjectWithTag("GameController");
            if (controller == null)
            {
                controller = new GameObject("Game Controller");
                controller.tag = "GameController";
                controller.AddComponent<ObjectPooler>();
                controller.AddComponent<GlobalEvents>();

                return true;
            }

            return false;
        }

        private bool SetUI()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("UI");
            if(canvas == null)
            {
                canvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Dias Games/Third Person System/Prefabs/UI.prefab");
                GameObject game = Instantiate(canvas);
                game.name = game.name.Replace("(Clone)", string.Empty);

                return true;
            }

            return false;
        }
    }
}
