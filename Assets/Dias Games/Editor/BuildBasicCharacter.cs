using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DiasGames.ThirdPersonSystem
{
    public class BuildBasicCharacter : EditorWindow
    {
        GameObject character; // Property to set character
        bool updateLayers; // Property to define layers

        PhysicMaterial zeroFriction;
        PhysicMaterial groundFriction;

        Animator m_Animator;

        [MenuItem("Tools/Third Person System/ Build Third Person Character")]
        public static void ShowWindow()
        {
            GetWindow<BuildBasicCharacter>(true, "Third Person Character Builder");
        }


        private void OnGUI()
        {
            GUILayout.Space(10);

            // Set label style for header
            GUIStyle m_Style = new GUIStyle(EditorStyles.label);
            m_Style.wordWrap = true;
            m_Style.fontStyle = FontStyle.Bold;


            // Draw GUI label and fields
            GUILayout.Label("This window will help you to create a Third Person Character. Start selecting your scene character", m_Style);

            character = EditorGUILayout.ObjectField("Character", character, typeof(GameObject), true) as GameObject;
            if (character == null)
            {
                EditorGUILayout.HelpBox("Please select a character to build.", MessageType.Error, false);
            }

            updateLayers = EditorGUILayout.Toggle("Update Project Layers", updateLayers);
            if (updateLayers == true)
                EditorGUILayout.HelpBox("Project layers will be overwritten. Be sure you want to do this.", MessageType.Warning, false);

            if (GUILayout.Button("Build Third Person Character") && character != null)
            {
                BuildCharacter();
                CreateRagdoll();
                Close();
                
            }
            ///

        }

        public void BuildCharacter()
        {
            if (updateLayers)
                UpdateLayers();

            zeroFriction = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Dias Games/Third Person System/Physics Materials/ZeroFriction.physicMaterial");
            groundFriction = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Dias Games/Third Person System/Physics Materials/GroundFriction.physicMaterial");

            #region Set Tag and Layer

            character.tag = "Player";
            character.layer = 15;

            #endregion

            #region Animator Builder
            m_Animator = character.GetComponent<Animator>();
            if (m_Animator == null)
                m_Animator = character.AddComponent<Animator>();

            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_Animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Dias Games/Third Person System/Animator/Third Person Animator.controller");
            #endregion

            #region Rigidbody Builder
            Rigidbody m_Rigidbody = character.GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = character.AddComponent<Rigidbody>();

            m_Rigidbody.angularDrag = 0;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            #endregion

            #region Capsule Collider Builder
            CapsuleCollider m_Capsule = character.GetComponent<CapsuleCollider>();
            if (m_Capsule == null)
                m_Capsule = character.AddComponent<CapsuleCollider>();

            m_Capsule.radius = 0.3f;
            m_Capsule.height = 1.8f;
            m_Capsule.center = new Vector3(0, m_Capsule.height * 0.5f, 0);
            m_Capsule.material = zeroFriction;
            #endregion

            #region Audio Builder

            AudioSource audio = character.GetComponent<AudioSource>();
            if (audio == null)
                audio = character.AddComponent<AudioSource>();
            audio.playOnAwake = false;

            #endregion

            #region Create Character Component

            ThirdPersonSystem m_Controller = character.GetComponent<ThirdPersonSystem>();
            if (m_Controller == null)
                m_Controller = character.AddComponent<ThirdPersonSystem>();

            m_Controller.GroundMask = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);

            HealthManager healthManager = character.GetComponent<HealthManager>();
            if (healthManager == null)
                healthManager = character.AddComponent<HealthManager>();

            #endregion

            AddAbilities();
        }

        void UpdateLayers()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layersProperty = tagManager.FindProperty("layers");

            AddNewLayer(layersProperty, 13, "Ragdoll");
            AddNewLayer(layersProperty, 14, "Ground");
            AddNewLayer(layersProperty, 15, "Character");

            tagManager.ApplyModifiedProperties();
        }

        private static void AddNewLayer(SerializedProperty property, int index, string name)
        {
            var propertyElement = property.GetArrayElementAtIndex(index);
            propertyElement.stringValue = name;
        }


        private void AddAbilities()
        {
            CameraData defaultCamera = AssetDatabase.LoadAssetAtPath<CameraData>("Assets/Dias Games/Third Person System/Camera Data/Default.asset");

            if (character.GetComponent<AnimatorManager>() == null)
                character.gameObject.AddComponent<AnimatorManager>();

            if (character.GetComponent<UnityInputManager>() == null)
                character.gameObject.AddComponent<UnityInputManager>();

            Locomotion m_Locomotion = character.GetComponent<Locomotion>();
            if (m_Locomotion == null)
            {
                m_Locomotion = character.gameObject.AddComponent<Locomotion>();

                m_Locomotion.AbilityPhysicMaterial = groundFriction;
                m_Locomotion.CameraData = defaultCamera;
            }

            JumpAbility m_Jump = character.GetComponent<JumpAbility>();
            if (m_Jump == null)
            {
                m_Jump = character.gameObject.AddComponent<JumpAbility>();
                m_Jump.AbilityPhysicMaterial = zeroFriction;
            }

            FallAbility m_Fall = character.GetComponent<FallAbility>();
            if (m_Fall == null)
            {
                m_Fall = character.gameObject.AddComponent<FallAbility>();
                m_Fall.AbilityPhysicMaterial = zeroFriction;
            }

            RollAbility m_Roll = character.GetComponent<RollAbility>();
            if (m_Roll == null)
                m_Roll = character.gameObject.AddComponent<RollAbility>();

            CrouchAbility m_Crouch = character.GetComponent<CrouchAbility>();
            if (m_Crouch == null)
            {
                m_Crouch = character.gameObject.AddComponent<CrouchAbility>();
                m_Crouch.AbilityPhysicMaterial = groundFriction;
                m_Crouch.CameraData = AssetDatabase.LoadAssetAtPath<CameraData>("Assets/Dias Games/Third Person System/Camera Data/CrouchCamera.asset");
            }

            
            // -------------------------- SET IGNORE ABILITIES ---------------------------------- //

            // Jump
            m_Jump.IgnoreAbilities = new List<ThirdPersonAbility>();
            m_Jump.IgnoreAbilities.Add(m_Locomotion);

            // Roll
            m_Roll.IgnoreAbilities = new List<ThirdPersonAbility>();
            m_Roll.IgnoreAbilities.Add(m_Locomotion);
            m_Roll.IgnoreAbilities.Add(m_Crouch);

            //Crouch
            m_Crouch.IgnoreAbilities = new List<ThirdPersonAbility>();
            m_Crouch.IgnoreAbilities.Add(m_Locomotion);

            // -------------------------------------------------------------------------------- //

        }

        private void CreateRagdoll()
        {
            var ragdollType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
            var windowsOpened = Resources.FindObjectsOfTypeAll(ragdollType);
            
            // Open Ragdoll window 
            if (windowsOpened == null || windowsOpened.Length == 0)
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                windowsOpened = Resources.FindObjectsOfTypeAll(ragdollType);
            }

            if (windowsOpened != null && windowsOpened.Length > 0)
            {
                ScriptableWizard ragdollWindow = windowsOpened[0] as ScriptableWizard;

                SetRagdollBoneValue(ragdollWindow, "pelvis", HumanBodyBones.Hips);
                SetRagdollBoneValue(ragdollWindow, "leftHips", HumanBodyBones.LeftUpperLeg);
                SetRagdollBoneValue(ragdollWindow, "leftKnee", HumanBodyBones.LeftLowerLeg);
                SetRagdollBoneValue(ragdollWindow, "leftFoot", HumanBodyBones.LeftFoot);
                SetRagdollBoneValue(ragdollWindow, "rightHips", HumanBodyBones.RightUpperLeg);
                SetRagdollBoneValue(ragdollWindow, "rightKnee", HumanBodyBones.RightLowerLeg);
                SetRagdollBoneValue(ragdollWindow, "rightFoot", HumanBodyBones.RightFoot);

                SetRagdollBoneValue(ragdollWindow, "leftArm", HumanBodyBones.LeftUpperArm);
                SetRagdollBoneValue(ragdollWindow, "leftElbow", HumanBodyBones.LeftLowerArm);
                SetRagdollBoneValue(ragdollWindow, "rightArm", HumanBodyBones.RightUpperArm);
                SetRagdollBoneValue(ragdollWindow, "rightElbow", HumanBodyBones.RightLowerArm);

                SetRagdollBoneValue(ragdollWindow, "middleSpine", HumanBodyBones.Spine);
                SetRagdollBoneValue(ragdollWindow, "head", HumanBodyBones.Head);
            }
        }

        private void SetRagdollBoneValue(ScriptableWizard window, string fieldName, HumanBodyBones bone)
        {
            var field = window.GetType().GetField(fieldName);
            field.SetValue(window, m_Animator.GetBoneTransform(bone));
            m_Animator.GetBoneTransform(bone).gameObject.layer = 13;
        }
    }
}