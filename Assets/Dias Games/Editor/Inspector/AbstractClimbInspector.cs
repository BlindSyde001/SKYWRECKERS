/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ThirdPersonAbstractClimbing), true)]
    public class AbstractClimbInspector : ThirdPersonAbilityInspector
    {
        // Abstract climbing properties
        SerializedProperty m_ClimbableMask;

        SerializedProperty m_CastCapsuleRadius;
        SerializedProperty m_Iterations;

        SerializedProperty m_VerticalLinecastStartPoint;
        SerializedProperty m_VerticalLinecastEndPoint;

        SerializedProperty m_UpdateCastByVerticalSpeed;
        SerializedProperty m_MaxDistanceToFindLedge;


        SerializedProperty m_VerticalDeltaFromLedge;
        SerializedProperty m_ForwardDeltaFromLedge;
        SerializedProperty m_PositioningSmoothnessTime;

        SerializedProperty gizmoColor; // The color that editor must draw on Scene
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            // FORWARD CAST PROPERTIES

            m_ClimbableMask = serializedObject.FindProperty("m_ClimbableMask");
            customProperties.Add(m_ClimbableMask.name);

            m_CastCapsuleRadius = serializedObject.FindProperty("m_CastCapsuleRadius");
            customProperties.Add(m_CastCapsuleRadius.name);

            m_Iterations = serializedObject.FindProperty("m_Iterations");
            customProperties.Add(m_Iterations.name);

            m_VerticalLinecastStartPoint = serializedObject.FindProperty("m_VerticalLinecastStartPoint");
            customProperties.Add(m_VerticalLinecastStartPoint.name);

            m_VerticalLinecastEndPoint = serializedObject.FindProperty("m_VerticalLinecastEndPoint");
            customProperties.Add(m_VerticalLinecastEndPoint.name);

            m_UpdateCastByVerticalSpeed = serializedObject.FindProperty("m_UpdateCastByVerticalSpeed");
            customProperties.Add(m_UpdateCastByVerticalSpeed.name);

            m_MaxDistanceToFindLedge = serializedObject.FindProperty("m_MaxDistanceToFindLedge");
            customProperties.Add(m_MaxDistanceToFindLedge.name);

            m_VerticalDeltaFromLedge = serializedObject.FindProperty("m_VerticalDeltaFromLedge");
            customProperties.Add(m_VerticalDeltaFromLedge.name);

            m_ForwardDeltaFromLedge = serializedObject.FindProperty("m_ForwardDeltaFromLedge");
            customProperties.Add(m_ForwardDeltaFromLedge.name);

            m_PositioningSmoothnessTime = serializedObject.FindProperty("m_PositioningSmoothnessTime");
            customProperties.Add(m_PositioningSmoothnessTime.name);
            
            m_CharacterAbilityTarget = target as ClimbingSystem.ThirdPersonAbstractClimbing;
        }

        protected override void FormatLabel()
        {
            base.FormatLabel();

            underLabel = "Climbing System";
        }

        protected override void DrawUniqueProperties()
        {
            base.DrawUniqueProperties();

            EditorGUILayout.Space();

            GUILayout.Label("Casting", contentSkin.label);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_ClimbableMask);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_MaxDistanceToFindLedge);
            EditorGUILayout.PropertyField(m_Iterations);
            EditorGUILayout.PropertyField(m_CastCapsuleRadius);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_VerticalLinecastStartPoint);
            EditorGUILayout.PropertyField(m_VerticalLinecastEndPoint);
            EditorGUILayout.PropertyField(m_UpdateCastByVerticalSpeed);

            // --------------------------- POSITIONING PARAMETERS ---------------------------------- //

            EditorGUILayout.Space();

            GUILayout.Label("Positioning", contentSkin.label);

            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(m_VerticalDeltaFromLedge);
            EditorGUILayout.PropertyField(m_ForwardDeltaFromLedge);
            EditorGUILayout.PropertyField(m_PositioningSmoothnessTime);

        }
    }
}
