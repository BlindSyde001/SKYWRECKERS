/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DiasGames.ThirdPersonSystem
{
    [CustomEditor(typeof(ThirdPersonAbility), true)]
    [CanEditMultipleObjects]
    public class ThirdPersonAbilityInspector : BaseInspector
    {
        // Serialized properties
        protected SerializedProperty m_EnterState;
        protected SerializedProperty m_TransitionDuration;
        SerializedProperty m_RootMotionMultiplier;
        protected SerializedProperty m_FinishOnAnimationEnd;
        SerializedProperty IgnoreAbilities;
        SerializedProperty m_UseRootMotion;
        SerializedProperty m_UseRotationRootMotion;
        SerializedProperty m_UseVerticalRootMotion;

        SerializedProperty allowZoomProperty;
        SerializedProperty customCameraProperty;
        SerializedProperty cameraData;

        SerializedProperty OnEnterAbilityEvent, OnExitAbilityEvent;

        // Internal vars
        protected DiasGames.ThirdPersonSystem.ThirdPersonAbility m_CharacterAbilityTarget;
        protected string m_EnterStateLabel = "Enter State";


        private bool drawAbilities = false;
        private bool drawEvent = false;
        private bool drawAnimation = false;
        private bool drawCamera = false;
        private bool customSettings = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_EnterState = serializedObject.FindProperty("m_EnterState");
            customProperties.Add(m_EnterState.name);

            m_TransitionDuration = serializedObject.FindProperty("m_TransitionDuration");
            customProperties.Add(m_TransitionDuration.name);

            m_FinishOnAnimationEnd = serializedObject.FindProperty("m_FinishOnAnimationEnd");
            customProperties.Add(m_FinishOnAnimationEnd.name);

            m_UseRootMotion = serializedObject.FindProperty("m_UseRootMotion");
            customProperties.Add(m_UseRootMotion.name);

            m_RootMotionMultiplier = serializedObject.FindProperty("m_RootMotionMultiplier");
            customProperties.Add(m_RootMotionMultiplier.name);

            m_UseRotationRootMotion = serializedObject.FindProperty("m_UseRotationRootMotion");
            customProperties.Add(m_UseRotationRootMotion.name);

            m_UseVerticalRootMotion = serializedObject.FindProperty("m_UseVerticalRootMotion");
            customProperties.Add(m_UseVerticalRootMotion.name);

            IgnoreAbilities = serializedObject.FindProperty("IgnoreAbilities");
            customProperties.Add(IgnoreAbilities.name);

            OnEnterAbilityEvent = serializedObject.FindProperty("OnEnterAbilityEvent");
            customProperties.Add(OnEnterAbilityEvent.name);

            OnExitAbilityEvent = serializedObject.FindProperty("OnExitAbilityEvent");
            customProperties.Add(OnExitAbilityEvent.name);


            allowZoomProperty = serializedObject.FindProperty("m_AllowCameraZoom");
            customProperties.Add(allowZoomProperty.name);

            customCameraProperty = serializedObject.FindProperty("m_CustomCameraData");
            customProperties.Add(customCameraProperty.name);

            cameraData = serializedObject.FindProperty("m_CameraData");
            customProperties.Add(cameraData.name);


            m_CharacterAbilityTarget = target as DiasGames.ThirdPersonSystem.ThirdPersonAbility;
            if (m_CharacterAbilityTarget.IgnoreAbilities == null)
                m_CharacterAbilityTarget.IgnoreAbilities = new List<ThirdPersonAbility>();

        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Ability Settings", (customSettings) ? active : normal))
                customSettings = !customSettings;

            if (customSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                DrawUniqueProperties();
                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Animation", (drawAnimation) ? active : normal))
                drawAnimation = !drawAnimation;

            if (drawAnimation)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                DrawAnimation();

                EditorGUILayout.Space();

                DrawRootMotionProperties();

                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Camera Settings", (drawCamera) ? active : normal))
                drawCamera = !drawCamera;

            if (drawCamera)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                DrawCameraSettings();
                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Abilities to ignore", (drawAbilities) ? active : normal))
                drawAbilities = !drawAbilities;

            if (drawAbilities)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                DrawIgnoreAbilities();
                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Events", (drawEvent) ? active : normal))
                drawEvent = !drawEvent;
            

            if (drawEvent)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                DrawEvents();
                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }


            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawAnimation()
        {
            EditorGUILayout.PropertyField(m_EnterState, new GUIContent(m_EnterStateLabel));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_TransitionDuration);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_FinishOnAnimationEnd);
        }

        protected void DrawIgnoreAbilities()
        {
            EditorGUI.indentLevel++;

            // -------------------------------------- ADD AND REMOVE ABILITIES BUTTONS -------------------------------------- //

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.HelpBox("Add or remove abilities to be ignored when this ability request to enter. " +
                "All abilities that are inside ignore abilities will be stopped to allow this ability to enter. " +
                "If Ignore abilities were void, this ability will enter only if noone ability is active", MessageType.Info);

            EditorGUILayout.Space();

            // AddButton Style
            GUIStyle addButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            addButtonStyle.alignment = TextAnchor.MiddleRight;
            addButtonStyle.fontStyle = FontStyle.Bold;
            addButtonStyle.fontSize = 13;
            addButtonStyle.stretchWidth = false;

            if (GUILayout.Button("+", addButtonStyle))
            {
                GenericMenu m_Menu = new GenericMenu();
                ThirdPersonAbility[] allAbilities = m_CharacterAbilityTarget.GetComponents<ThirdPersonAbility>();
                foreach (ThirdPersonAbility ability in allAbilities)
                {
                    if (m_CharacterAbilityTarget.IgnoreAbilities.Contains(ability) || ability == m_CharacterAbilityTarget)
                        continue;

                    m_Menu.AddItem(new GUIContent(ability.GetType().Name), false, AddIgnoredAbility, ability);
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
                foreach (ThirdPersonAbility ability in m_CharacterAbilityTarget.IgnoreAbilities)
                {
                    m_Menu.AddItem(new GUIContent(ability.GetType().Name), false, RemoveIgnoredAbility, ability);
                }

                m_Menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // ----------------------------------------------------------------------------------------------------- //



            // Draw ignored abilities
            for (int i = 0; i < m_CharacterAbilityTarget.IgnoreAbilities.Count; i++)
            {
                ThirdPersonAbility ability = m_CharacterAbilityTarget.IgnoreAbilities[i];
                if (ability == null)
                {
                    m_CharacterAbilityTarget.IgnoreAbilities.RemoveAt(i);
                    break;
                }
                EditorGUILayout.PropertyField(IgnoreAbilities.GetArrayElementAtIndex(i), new GUIContent(ability.GetType().Name));
            }

            EditorGUI.indentLevel--;
        }

        protected virtual void DrawRootMotionProperties()
        {
            EditorGUILayout.PropertyField(m_UseRootMotion);
            if (m_CharacterAbilityTarget.UseRootMotion)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_UseVerticalRootMotion);
                EditorGUILayout.PropertyField(m_UseRotationRootMotion);

                EditorGUILayout.Space();

                DrawRootMotionMultiplier();

                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DrawRootMotionMultiplier()
        {
            EditorGUILayout.PropertyField(m_RootMotionMultiplier);
        }

        protected void DrawCameraSettings()
        {
            EditorGUILayout.PropertyField(allowZoomProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(customCameraProperty);
            if (customCameraProperty.boolValue)
            {
                EditorGUILayout.PropertyField(cameraData);

                if (cameraData.objectReferenceValue != null)
                {
                    EditorGUI.indentLevel++;

                    SerializedObject cameraObject = new SerializedObject(cameraData.objectReferenceValue);

                    cameraObject.Update();

                    SerializedProperty offset = cameraObject.FindProperty("Offset");
                    SerializedProperty field = cameraObject.FindProperty("FieldOfView");

                    EditorGUILayout.PropertyField(offset);
                    EditorGUILayout.PropertyField(field);

                    cameraObject.ApplyModifiedProperties();

                    EditorGUI.indentLevel--;
                }
            }
        }

        protected virtual void DrawUniqueProperties()
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

        protected void DrawEvents()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(OnEnterAbilityEvent);
            EditorGUILayout.PropertyField(OnExitAbilityEvent);

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Add a new ability to be ignored by a Context menu
        /// </summary>
        /// <param name="newAbility"></param>
        void AddIgnoredAbility(object newAbility)
        {
            int i = IgnoreAbilities.arraySize;
            IgnoreAbilities.arraySize++;
            IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue = newAbility as ThirdPersonAbility;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Remove an ignored ability from the list
        /// </summary>
        /// <param name="currentAbility"></param>
        void RemoveIgnoredAbility(object currentAbility)
        {
            int index = -1;
            ThirdPersonAbility abilityToRemove = currentAbility as ThirdPersonAbility;
            for(int i=0; i < IgnoreAbilities.arraySize; i++)
            {
                if(IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue == abilityToRemove)
                {
                    index = i;
                    break;
                }
            }

            IgnoreAbilities.DeleteArrayElementAtIndex(index);
            if(IgnoreAbilities.GetArrayElementAtIndex(index).objectReferenceValue == null)
                IgnoreAbilities.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();

        }

    }
}
