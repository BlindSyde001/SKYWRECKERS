/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEditor;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    [CustomEditor(typeof(ClimbingSystem.ClimbLadderAbility))]
    [CanEditMultipleObjects]
    public class ClimbLadderInspector : ThirdPersonAbilityInspector
    {
        SerializedProperty m_LadderClimbUp;
        SerializedProperty m_LadderJumpUp;
        
        SerializedProperty m_MultiplierOnJump;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            m_EnterStateLabel = "Ladder State";

            m_LadderClimbUp = serializedObject.FindProperty("m_LadderClimbUp");
            customProperties.Add(m_LadderClimbUp.name);

            m_LadderJumpUp = serializedObject.FindProperty("m_LadderJumpUp");
            customProperties.Add(m_LadderJumpUp.name);
                        
            m_MultiplierOnJump = serializedObject.FindProperty("m_MultiplierOnJump");
            customProperties.Add(m_MultiplierOnJump.name);

        }

        protected override void FormatLabel()
        {
            label = "Ladder";
            underLabel = "Climbing System";
        }

        protected override void DrawAnimation()
        {
            base.DrawAnimation();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_LadderClimbUp);
            EditorGUILayout.PropertyField(m_LadderJumpUp);
        }

        protected override void DrawRootMotionMultiplier()
        {
            base.DrawRootMotionMultiplier();

            EditorGUILayout.PropertyField(m_MultiplierOnJump);
        }
    }
}
