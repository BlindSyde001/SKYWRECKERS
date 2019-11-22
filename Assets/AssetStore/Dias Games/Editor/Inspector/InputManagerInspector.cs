using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityInputManager))]
    public class InputManagerInspector : BaseInspector
    {
        private bool hasWeaponController = false;
        private bool showInput = true;

        private SerializedProperty camera;

        protected override void OnEnable()
        {
            base.OnEnable();

            camera = serializedObject.FindProperty("m_Camera");

            Behaviour[] behaviours = (serializedObject.targetObject as MonoBehaviour).GetComponents<Behaviour>();
            foreach(Behaviour behaviour in behaviours)
            {
                if(behaviour.GetType().Name.Contains("WeaponController"))
                {
                    hasWeaponController = true;
                    break;
                }
            }


        }

        protected override void FormatLabel()
        {
            label = "Input Manager";
            underLabel = "Third Person System";
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(camera);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            EditorGUI.indentLevel++;

            if (GUILayout.Button("Input Buttons", (showInput) ? active : normal))
                showInput = !showInput;

            if (showInput)
            {
                EditorGUILayout.Space();

                SerializedProperty property = serializedObject.GetIterator();
                if (property.NextVisible(true))
                {
                    do
                    {
                        if (property.name.Contains("m_Script") || property.name.Contains("m_Camera"))
                            continue;

                        if ((property.name.Contains("Weapon") || property.name.Contains("Fire") || property.name.Contains("Reload")))
                              break;

                        EditorGUILayout.PropertyField(property, true);

                    } while (property.NextVisible(false));
                }

                EditorGUILayout.Space();

                if (hasWeaponController)
                {
                    GUILayout.Label("Weapon Input", contentSkin.label);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(property, true);
                    if (property.NextVisible(true))
                    {
                        do
                        {
                            if ((property.name.Contains("Weapon") || property.name.Contains("Fire") || property.name.Contains("Reload"))) 
                                EditorGUILayout.PropertyField(property, true);

                        } while (property.NextVisible(false));
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
