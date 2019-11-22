/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEditor;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    [CustomEditor(typeof(ClimbingSystem.ClimbIK))]
    [CanEditMultipleObjects]
    public class ClimbIKInspector : BaseInspector
    {
        SerializedProperty m_RunHandIK;
        SerializedProperty m_RunFootIK;
        SerializedProperty m_RunBodyIK;

        SerializedProperty m_DistanceToCast;

        SerializedProperty m_HandHorOffset;
        SerializedProperty m_HandVertOffset;
        SerializedProperty m_HandCenterOffset;

        SerializedProperty m_HandCapsuleRadius;
        SerializedProperty m_HandCapsuleHeight;

        SerializedProperty m_HandDistanceFromFeet;
        SerializedProperty m_HandIKSmooth;

        SerializedProperty m_FeetLayers;
        SerializedProperty m_RightFootWallOfsset;
        SerializedProperty m_LeftFootWallOfsset;

        SerializedProperty m_BodyAdjusmentOnHang;
        
        private bool showHand, showFoot, showBody;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_RunHandIK = serializedObject.FindProperty("m_RunHandIK");
            m_RunFootIK = serializedObject.FindProperty("m_RunFootIK");
            m_RunBodyIK = serializedObject.FindProperty("m_RunBodyIK");

            m_DistanceToCast = serializedObject.FindProperty("m_DistanceToCast");

            m_HandHorOffset = serializedObject.FindProperty("m_HandHorOffset");
            m_HandVertOffset = serializedObject.FindProperty("m_HandVertOffset");
            m_HandCenterOffset = serializedObject.FindProperty("m_HandCenterOffset");
            m_HandCapsuleRadius = serializedObject.FindProperty("m_HandCapsuleRadius");
            m_HandCapsuleHeight = serializedObject.FindProperty("m_HandCapsuleHeight");
            m_HandDistanceFromFeet = serializedObject.FindProperty("m_HandDistanceFromFeet");
            m_HandIKSmooth = serializedObject.FindProperty("m_HandIKSmooth");

            m_FeetLayers = serializedObject.FindProperty("m_FeetLayers");
            m_RightFootWallOfsset = serializedObject.FindProperty("m_RightFootWallOfsset");
            m_LeftFootWallOfsset = serializedObject.FindProperty("m_LeftFootWallOfsset");

            m_BodyAdjusmentOnHang = serializedObject.FindProperty("m_BodyAdjusmentOnHang");

        }

        protected override void FormatLabel()
        {
            underLabel = "Climbing System";
            label = "Climb IK";
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            GUIStyle normal = new GUIStyle(contentSkin.button);
            GUIStyle active = new GUIStyle(contentSkin.button);
            active.normal.background = active.active.background;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_DistanceToCast);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);

            if (GUILayout.Button("Hand IK", (showHand) ? active : normal))
                showHand = !showHand;

            if (showHand)
            {
                EditorGUILayout.Space();
                DrawHand();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            if (GUILayout.Button("Foot IK", (showFoot) ? active : normal))
                showFoot = !showFoot;

            if (showFoot)
            {
                EditorGUILayout.Space();
                DrawFeet();
                EditorGUILayout.Space();

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            if (GUILayout.Button("Body IK For Hanging", (showBody) ? active : normal))
                showBody = !showBody;

            if (showBody)
            {
                EditorGUILayout.Space();
                DrawBody();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawHand()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_RunHandIK);
            if (m_RunHandIK.boolValue)
            {
                EditorGUILayout.PropertyField(m_HandCapsuleRadius);
                EditorGUILayout.PropertyField(m_HandCapsuleHeight);
                EditorGUILayout.PropertyField(m_HandHorOffset, new GUIContent("Hand Forward Offset"));
                EditorGUILayout.PropertyField(m_HandVertOffset, new GUIContent("Hand Vertical Offset"));
                EditorGUILayout.PropertyField(m_HandCenterOffset, new GUIContent("Hand Horizontal Offset"));
                EditorGUILayout.PropertyField(m_HandDistanceFromFeet);
                EditorGUILayout.PropertyField(m_HandIKSmooth);
            }

            EditorGUI.indentLevel--;
        }

        void DrawFeet()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_RunFootIK);
            if (m_RunFootIK.boolValue)
            {
                EditorGUILayout.PropertyField(m_FeetLayers);
                EditorGUILayout.PropertyField(m_RightFootWallOfsset);
                EditorGUILayout.PropertyField(m_LeftFootWallOfsset);
            }

            EditorGUI.indentLevel--;
        }

        void DrawBody()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_RunBodyIK);
            if (m_RunBodyIK.boolValue)
                EditorGUILayout.PropertyField(m_BodyAdjusmentOnHang);

            EditorGUI.indentLevel--;
        }
    }
}
