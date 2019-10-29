using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class BaseInspector : Editor
    {
        protected Texture2D icon;
        protected GUISkin headerSkin;
        protected GUISkin contentSkin;

        protected string underLabel = "Third Person System";
        protected string label = "";

        protected GUIStyle normal;
        protected GUIStyle active;

        protected List<string> customProperties = new List<string>();

        protected virtual void OnEnable()
        {
            icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Dias Games/Editor/GUI/tps_icon.psd", typeof(Texture2D));
            headerSkin = Resources.Load("HeaderSkin") as GUISkin;
            contentSkin = Resources.Load("ContentSkin") as GUISkin;

            FormatLabel();

            normal = new GUIStyle(contentSkin.button);
            active = new GUIStyle(contentSkin.button);
            active.normal.background = active.active.background;
        }

        protected virtual void FormatLabel()
        {
            label = serializedObject.targetObject.GetType().Name;

            if (label.Contains("Ability"))
                label = label.Remove(label.IndexOf("Ability"), "Ability".Length);

            for (int i = 0; i < label.Length; i++)
            {
                char letter = label[i];
                if (char.IsUpper(letter) && i > 0)
                {
                    label = label.Insert(i, " ");
                    break;
                }
            }
        }

        protected void DrawImageHeader()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(headerSkin.box);
            GUILayout.Label(icon);

            EditorGUILayout.BeginVertical();
            GUILayout.Label(label, headerSkin.label);
            GUILayout.Label(underLabel, headerSkin.textArea);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }


        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            base.OnInspectorGUI();
        }
    }
}