using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Cameras.CameraController))]
    public class CameraControllerInspector : BaseInspector
    {
        SerializedProperty followTarget;
        SerializedProperty autoTarget;
        SerializedProperty updateMode;

        SerializedProperty m_MoveSpeed;    
        SerializedProperty m_TurnSpeed;                     
        SerializedProperty m_TurnSmoothing;            
        SerializedProperty m_MaxPitch;                      
        SerializedProperty m_MinPitch;                    
        SerializedProperty m_HideCursor;

        SerializedProperty m_TransitionSmooth;

        SerializedProperty TurnXInput;
        SerializedProperty TurnYInput;

        private bool showMove = true;
        private bool showClip = false;

        SerializedProperty m_ClipLayers;
        SerializedProperty clipSpeed;
        SerializedProperty sphereCastRadius;

        private bool showData = false;

        SerializedProperty m_DefaultCameraData;            
        SerializedProperty m_DefaultZoomCameraData;       

        protected override void OnEnable()
        {
            base.OnEnable();

            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Dias Games/Editor/GUI/tps_cam_icon.psd");

            followTarget = serializedObject.FindProperty("m_Target");
            customProperties.Add(followTarget.name);

            autoTarget = serializedObject.FindProperty("m_AutoTargetPlayer");
            customProperties.Add(autoTarget.name);

            updateMode = serializedObject.FindProperty("m_UpdateType");
            customProperties.Add(updateMode.name);


            m_MoveSpeed = serializedObject.FindProperty("m_MoveSpeed");
            customProperties.Add(m_MoveSpeed.name);

            m_TurnSpeed = serializedObject.FindProperty("m_TurnSpeed");
            customProperties.Add(m_TurnSpeed.name);

            m_TurnSmoothing = serializedObject.FindProperty("m_TurnSmoothing");
            customProperties.Add(m_TurnSmoothing.name);

            m_MaxPitch = serializedObject.FindProperty("m_MaxPitch");
            customProperties.Add(m_MaxPitch.name);

            m_MinPitch = serializedObject.FindProperty("m_MinPitch");
            customProperties.Add(m_MinPitch.name);

            m_HideCursor = serializedObject.FindProperty("m_HideCursor");
            customProperties.Add(m_HideCursor.name);

            m_TransitionSmooth = serializedObject.FindProperty("m_TransitionSmooth");
            customProperties.Add(m_TransitionSmooth.name);

            TurnXInput = serializedObject.FindProperty("TurnXInput");
            customProperties.Add(TurnXInput.name);

            TurnYInput = serializedObject.FindProperty("TurnYInput");
            customProperties.Add(TurnYInput.name);


            m_ClipLayers = serializedObject.FindProperty("m_ClipLayers");
            customProperties.Add(m_ClipLayers.name);

            sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
            customProperties.Add(sphereCastRadius.name);

            clipSpeed = serializedObject.FindProperty("clipSpeed");
            customProperties.Add(clipSpeed.name);


            m_DefaultCameraData = serializedObject.FindProperty("m_DefaultCameraData");
            customProperties.Add(m_DefaultCameraData.name);

            m_DefaultZoomCameraData = serializedObject.FindProperty("m_DefaultZoomCameraData");
            customProperties.Add(m_DefaultZoomCameraData.name);
        }

        protected override void FormatLabel()
        {
            label = "Camera Controller";
            underLabel = "Third Person System";
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            
            if (GUILayout.Button("Camera Movement Settings", (showMove) ? active : normal))
                showMove = !showMove;

            if (showMove)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                DrawMovementSettings();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Camera Protection From Clip", (showClip) ? active : normal))
                showClip = !showClip;

            if (showClip)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                DrawClipSettings();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Camera Data", (showData) ? active : normal))
                showData = !showData;

            if (showData)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                DrawCameraDataSettings();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMovementSettings()
        {
            EditorGUILayout.PropertyField(updateMode);
            EditorGUILayout.PropertyField(autoTarget);
            if (!autoTarget.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(followTarget);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_MoveSpeed);
            EditorGUILayout.PropertyField(m_TurnSpeed);
            EditorGUILayout.PropertyField(m_TurnSmoothing);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_MaxPitch);
            EditorGUILayout.PropertyField(m_MinPitch);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_HideCursor, new GUIContent("Hide and Lock Cursor"));

            EditorGUILayout.Space();

            GUILayout.Label("Input", contentSkin.label);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(TurnXInput, new GUIContent("Horizontal Turn Input"));
            EditorGUILayout.PropertyField(TurnYInput, new GUIContent("Vertical Turn Input"));
        }

        private void DrawClipSettings()
        {
            EditorGUILayout.PropertyField(m_ClipLayers);
            EditorGUILayout.PropertyField(sphereCastRadius);
            EditorGUILayout.PropertyField(clipSpeed);
        }

        private void DrawCameraDataSettings()
        {
            EditorGUILayout.PropertyField(m_TransitionSmooth);

            EditorGUILayout.Space();

            GUIStyle box = new GUIStyle(contentSkin.box);
            box.margin = new RectOffset(15, 15, 0, 0);
            box.padding.bottom = 15;

            EditorGUILayout.BeginVertical(box);

            GUILayout.Label("Default Camera Data", contentSkin.label);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_DefaultCameraData, new GUIContent("Default Data"));
            DrawInternalCameraData(m_DefaultCameraData);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(box);

            GUILayout.Label("Zoom Camera Data", contentSkin.label);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_DefaultZoomCameraData, new GUIContent("Zoom Data"));
            DrawInternalCameraData(m_DefaultZoomCameraData);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        private void DrawInternalCameraData(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;

                SerializedObject cameraObject = new SerializedObject(property.objectReferenceValue);

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
}