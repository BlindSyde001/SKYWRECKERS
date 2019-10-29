/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    [CustomEditor(typeof(ClimbingAbility))]
    [CanEditMultipleObjects]
    public class ClimbingInspector : AbstractClimbInspector
    {

        // ---------------------------------------------------- SIDE CLIMBING PARAMETERS ------------------------------------------------ //

        SerializedProperty m_SideCapsuleRadius;
        SerializedProperty m_SideCapsuleHeight;
        SerializedProperty m_SideMaxDistanceToCast;

        SerializedProperty m_SideDistanceFromCharacter;

        SerializedProperty m_CharacterOffsetOnSide;

        SerializedProperty m_TimeToTurnLedge;

        // ------------------------------------------------------------------------------------------------------------------------------ //
        
        protected override void OnEnable()
        {
            base.OnEnable();
                       
            // SIDE CAST PROPERTTIES

            m_TimeToTurnLedge = serializedObject.FindProperty("m_TimeToTurnLedge");
            customProperties.Add(m_TimeToTurnLedge.name);


            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("StateName") || property.name.Contains("Multiplier") || property.name.Contains("Side"))
                        customProperties.Add(property.name);

                } while (property.NextVisible(false));
            }
        }



        protected override void DrawAnimation()
        {
            base.DrawAnimation();

            EditorGUILayout.Space();

            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("StateName"))
                        EditorGUILayout.PropertyField(property);

                } while (property.NextVisible(false));
            }
        }

        protected override void DrawRootMotionMultiplier()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("Multiplier"))
                        EditorGUILayout.PropertyField(property);

                } while (property.NextVisible(false));
            }
        }

        protected override void DrawUniqueProperties()
        {
            base.DrawUniqueProperties();

            EditorGUILayout.Space();

            GUILayout.Label("Side Casting", contentSkin.label);

            EditorGUILayout.Space();

            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("Side") && !property.name.Contains("Multiplier"))
                        EditorGUILayout.PropertyField(property);

                } while (property.NextVisible(false));
            }

            EditorGUILayout.PropertyField(m_TimeToTurnLedge);
        }
    }
}
