/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CustomEditor(typeof(ThirdPersonSystem))]
    [CanEditMultipleObjects]
    public class ThirdPersonEngineInspector : Editor
    {
        SerializedProperty m_GroundMask;
        SerializedProperty m_GroundCheckDistance;
        SerializedProperty m_MaxAngleSlope;

        SerializedProperty m_ApplyExtraGravity;
        SerializedProperty m_GravityMultiplier;

        SerializedProperty alwaysZoomProperty;

        SerializedProperty OnAnyAbilityEnters, OnAnyAbilityExits;

        SerializedProperty m_BehavioursToDisable;

        List<string> customProperties = new List<string>();

        // Internal
        bool showEvents = false;
        private bool showSettings = true;
        private bool showDisableScripts = false;

        ThirdPersonSystem m_Controller;

        private Texture2D icon;
        private GUISkin customSkin;
        private GUISkin contentSkin;
        
        private void OnEnable()
        {
            m_GroundMask = serializedObject.FindProperty("m_GroundMask");
            customProperties.Add(m_GroundMask.name);

            m_GroundCheckDistance = serializedObject.FindProperty("m_GroundCheckDistance");
            customProperties.Add(m_GroundCheckDistance.name);

            m_MaxAngleSlope = serializedObject.FindProperty("m_MaxAngleSlope");
            customProperties.Add(m_MaxAngleSlope.name);



            m_ApplyExtraGravity = serializedObject.FindProperty("m_ApplyExtraGravity");
            customProperties.Add(m_ApplyExtraGravity.name);

            m_GravityMultiplier = serializedObject.FindProperty("m_GravityMultiplier");
            customProperties.Add(m_GravityMultiplier.name);



            alwaysZoomProperty = serializedObject.FindProperty("m_AlwaysZoomCamera");
            customProperties.Add(alwaysZoomProperty.name);
            


            OnAnyAbilityEnters = serializedObject.FindProperty("OnAnyAbilityEnters");
            customProperties.Add(OnAnyAbilityEnters.name);

            OnAnyAbilityExits = serializedObject.FindProperty("OnAnyAbilityExits");
            customProperties.Add(OnAnyAbilityExits.name);


            m_BehavioursToDisable = serializedObject.FindProperty("m_BehavioursToDisable");
            customProperties.Add(m_BehavioursToDisable.name);

            m_Controller = target as global::DiasGames.ThirdPersonSystem.ThirdPersonSystem;

            icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Dias Games/Editor/GUI/tps_icon.psd", typeof(Texture2D));
            customSkin = Resources.Load("HeaderSkin") as GUISkin;
            contentSkin = Resources.Load("ContentSkin") as GUISkin;
        }

        public override void OnInspectorGUI()
        {
            #region Draw Image Header
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(customSkin.box);
            GUILayout.Label(icon);

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Main Controller", customSkin.label);
            GUILayout.Label("Third Person System", customSkin.textArea);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            #endregion

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            GUIStyle normal = new GUIStyle(contentSkin.button);
            GUIStyle active = new GUIStyle(contentSkin.button);
            active.normal.background = active.active.background;

            if (GUILayout.Button("Controller Settings", (showSettings) ? active : normal))
                showSettings = !showSettings;

            if (showSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_GroundMask);
                EditorGUILayout.PropertyField(m_GroundCheckDistance);
                EditorGUILayout.PropertyField(m_MaxAngleSlope);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_ApplyExtraGravity);
                EditorGUILayout.PropertyField(m_GravityMultiplier);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(alwaysZoomProperty);
                
                EditorGUILayout.Space();

                // Draw all other properties
                DrawExtraParameters();

                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal(contentSkin.box);

                EditorGUILayout.LabelField("Current Active Ability");

                string activeAbility = (m_Controller.ActiveAbility == null) ? "Null" : m_Controller.ActiveAbility.GetType().Name;
                EditorGUILayout.LabelField(activeAbility, EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            DrawEventsRegion(normal, active);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            if (GUILayout.Button("Disable scripts", (showDisableScripts) ? active : normal))
                showDisableScripts = !showDisableScripts;

            if (showDisableScripts)
            {
                EditorGUILayout.Space();

                DrawBehavioursToDisable();

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawExtraParameters()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (!customProperties.Contains(property.name) && !property.name.Contains("Script"))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
        }

        private void DrawEventsRegion(GUIStyle normal, GUIStyle active)
        {
            if (GUILayout.Button("Events", (showEvents) ? active : normal))
                showEvents = !showEvents;

            if (showEvents)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(OnAnyAbilityEnters);
                EditorGUILayout.PropertyField(OnAnyAbilityExits);

                EditorGUILayout.Space();
                
                EditorGUI.indentLevel--;
            }
        }

        protected void DrawBehavioursToDisable()
        {
            EditorGUI.indentLevel++;

            // -------------------------------------- ADD AND REMOVE ABILITIES BUTTONS -------------------------------------- //

            EditorGUILayout.BeginHorizontal();

            // Draw header
            GUIStyle header = new GUIStyle(EditorStyles.boldLabel);
            header.alignment = TextAnchor.MiddleCenter;
            header.fontSize = 12;
            header.fixedHeight = 20f;
            EditorGUILayout.LabelField("Disable scripts", header);

            // AddButton Style
            GUIStyle addButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            addButtonStyle.alignment = TextAnchor.MiddleRight;
            addButtonStyle.fontStyle = FontStyle.Bold;
            addButtonStyle.fontSize = 13;
            addButtonStyle.stretchWidth = false;

            if (GUILayout.Button("+", addButtonStyle))
            {
                GenericMenu m_Menu = new GenericMenu();
                Behaviour[] allBehaviours = m_Controller.GetComponents<Behaviour>();
                foreach (Behaviour behaviour in allBehaviours)
                {
                    if (m_Controller.m_BehavioursToDisable.Contains(behaviour) || behaviour == m_Controller)
                        continue;

                    m_Menu.AddItem(new GUIContent(behaviour.GetType().Name), false, AddBehaviourToDisable, behaviour);
                }

                m_Menu.ShowAsContext();
            }

            // RemoveButton Style
            GUIStyle removeButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);
            removeButtonStyle.alignment = TextAnchor.MiddleCenter;
            removeButtonStyle.fontStyle = FontStyle.Bold;
            removeButtonStyle.fontSize = 13;
            removeButtonStyle.stretchWidth = false;

            if (GUILayout.Button("-", removeButtonStyle))
            {
                GenericMenu m_Menu = new GenericMenu();
                foreach (Behaviour behaviour in m_Controller.m_BehavioursToDisable)
                {
                    m_Menu.AddItem(new GUIContent(behaviour.GetType().Name), false, RemoveBehaviourToDisable, behaviour);
                }

                m_Menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // ----------------------------------------------------------------------------------------------------- //


            EditorGUILayout.HelpBox("All scripts in this list will be disable when any ability is active.", MessageType.Info);

            EditorGUILayout.Space();

            // Draw ignored abilities

            for (int i = 0; i < m_Controller.m_BehavioursToDisable.Count; i++)
            {
                Behaviour behaviour = m_Controller.m_BehavioursToDisable[i];
                if (behaviour == null)
                {
                    m_Controller.m_BehavioursToDisable.RemoveAt(i);
                    break;
                }
                EditorGUILayout.PropertyField(m_BehavioursToDisable.GetArrayElementAtIndex(i), new GUIContent(behaviour.GetType().Name));
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Add a new behaviour by a Context menu
        /// </summary>
        /// <param name="newAbility"></param>
        void AddBehaviourToDisable(object newAbility)
        {
            m_Controller.m_BehavioursToDisable.Add(newAbility as Behaviour);
        }

        /// <summary>
        /// Remove an behaviour from the list
        /// </summary>
        /// <param name="currentAbility"></param>
        void RemoveBehaviourToDisable(object currentAbility)
        {
            m_Controller.m_BehavioursToDisable.Remove(currentAbility as Behaviour);
        }
    }
}
