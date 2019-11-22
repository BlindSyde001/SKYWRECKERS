using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using System;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class BuildClimbingCharacter : EditorWindow
    {
        GameObject character; // Property to set character
        bool updateLayers; // Property to define layers

        PhysicMaterial zeroFriction;
        PhysicMaterial groundFriction;

        Animator m_Animator;

        [MenuItem("Tools/Third Person System/Build Climbing Character")]
        public static void ShowWindow()
        {
            GetWindow<BuildClimbingCharacter>(true, "Climbing Character Builder");
        }


        private void OnGUI()
        {
            GUILayout.Space(10);

            // Set label style for header
            GUIStyle m_Style = new GUIStyle(EditorStyles.label);
            m_Style.wordWrap = true;
            m_Style.fontStyle = FontStyle.Bold;


            // Draw GUI label and fields
            GUILayout.Label("This window will help you to create your character for Climbing System. Start selecting your scene character", m_Style);

            character = EditorGUILayout.ObjectField("Character", character, typeof(GameObject), true) as GameObject;
            if (character == null)
            {
                EditorGUILayout.HelpBox("Please select a character to build.", MessageType.Error, false);
            }
            
            if (GUILayout.Button("Build Climbing Character") && character != null)
            {
                BuildCharacter();
                CreateRagdoll();

                Close();
            }
            ///

        }

        public void BuildCharacter()
        {
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

            m_Animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Dias Games/Climbing System/Animator/Climbing Animator.controller");
            
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

        
        private void AddAbilities()
        {
            CameraData defaultCamera = AssetDatabase.LoadAssetAtPath<CameraData>("Assets/Dias Games/Third Person System/Camera Data/Default.asset");
            CameraData climbingCamera = AssetDatabase.LoadAssetAtPath<CameraData>("Assets/Dias Games/Climbing System/Camera Data/ClimbingCamera.asset");
            CameraData ladderCamera = AssetDatabase.LoadAssetAtPath<CameraData>("Assets/Dias Games/Climbing System/Camera Data/LadderCamera.asset");

            if (character.GetComponent<DiasGames.ThirdPersonSystem.AnimatorManager>() == null)
                character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.AnimatorManager>();

            if (character.GetComponent<DiasGames.ThirdPersonSystem.UnityInputManager>() == null)
                character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.UnityInputManager>();

            DiasGames.ThirdPersonSystem.Locomotion m_Locomotion = character.GetComponent<DiasGames.ThirdPersonSystem.Locomotion>();
            if (m_Locomotion == null)
            {
                m_Locomotion = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.Locomotion>();

                m_Locomotion.AbilityPhysicMaterial = zeroFriction;
                m_Locomotion.CameraData = defaultCamera;
            }

            DiasGames.ThirdPersonSystem.JumpAbility m_Jump = character.GetComponent<DiasGames.ThirdPersonSystem.JumpAbility>();
            if (m_Jump == null)
            {
                m_Jump = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.JumpAbility>();
                m_Jump.AbilityPhysicMaterial = zeroFriction;
            }

            DiasGames.ThirdPersonSystem.FallAbility m_Fall = character.GetComponent<DiasGames.ThirdPersonSystem.FallAbility>();
            if (m_Fall == null)
            {
                m_Fall = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.FallAbility>();
                m_Fall.AbilityPhysicMaterial = zeroFriction;
            }

            DiasGames.ThirdPersonSystem.RollAbility m_Roll = character.GetComponent<DiasGames.ThirdPersonSystem.RollAbility>();
            if (m_Roll == null)
                m_Roll = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.RollAbility>();

            DiasGames.ThirdPersonSystem.CrouchAbility m_Crouch = character.GetComponent<DiasGames.ThirdPersonSystem.CrouchAbility>();
            if (m_Crouch == null)
            {
                m_Crouch = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.CrouchAbility>();
                m_Crouch.AbilityPhysicMaterial = groundFriction;
            }

            LayerMask m_FootLayers = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);

            // Vault ability
            DiasGames.ThirdPersonSystem.ClimbingSystem.VaultAbility m_Vault = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.VaultAbility>();
            if (m_Vault == null)
                m_Vault = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.VaultAbility>();

            m_Vault.ClimbingMask = (1 << 21);
            ////
            
            // Step Up ability Setup
            DiasGames.ThirdPersonSystem.ClimbingSystem.StepUpAbility m_StepUp = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.StepUpAbility>();
            if (m_StepUp == null)
                m_StepUp = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.StepUpAbility>();

            m_StepUp.ClimbingMask = (1 << 16);
            ////

            // Half Climb ability Setup
            DiasGames.ThirdPersonSystem.ClimbingSystem.LowerClimbAbility m_LowerClimb = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.LowerClimbAbility>();
            if (m_LowerClimb == null)
                m_LowerClimb = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.LowerClimbAbility>();

            m_LowerClimb.ClimbingMask = (1 << 17);
            ////

            // Climbing ability setup
            DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbingAbility m_Climbing = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbingAbility>();
            if (m_Climbing == null)
                m_Climbing = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbingAbility>();

            m_Climbing.ClimbingMask = (1 << 18) | (1 << 19);
            m_Climbing.CameraData = climbingCamera;
            ////

            DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK m_ClimbIK = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK>();
            if (m_ClimbIK == null)
                m_ClimbIK = character.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK>();

            m_ClimbIK.m_FeetLayers = m_FootLayers;

            DiasGames.ThirdPersonSystem.ClimbingSystem.DropToClimbAbility m_DropToClimb = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.DropToClimbAbility>();
            if (m_DropToClimb == null)
                m_DropToClimb = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.DropToClimbAbility>();

            m_DropToClimb.DroppableLayer = (1 << 19) | (1 << 20);
            m_DropToClimb.CameraData = climbingCamera;

            DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbLadderAbility m_Ladder = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbLadderAbility>();
            if (m_Ladder == null)
                m_Ladder = character.gameObject.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbLadderAbility>();
            m_Ladder.CameraData = ladderCamera;


            // -------------------------- SET IGNORE ABILITIES ---------------------------------- //

            // Jump
            m_Jump.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_Jump.IgnoreAbilities.Add(m_Locomotion);

            // Roll
            m_Roll.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_Roll.IgnoreAbilities.Add(m_Locomotion);
            m_Roll.IgnoreAbilities.Add(m_Crouch);

            //Crouch
            m_Crouch.IgnoreAbilities = new List<ThirdPersonAbility>();
            m_Crouch.IgnoreAbilities.Add(m_Locomotion);

            // Climb ladder
            m_Ladder.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_Ladder.IgnoreAbilities.Add(m_Locomotion);
            m_Ladder.IgnoreAbilities.Add(m_Fall);
            m_Ladder.IgnoreAbilities.Add(m_Jump);

            // Step up
            m_StepUp.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_StepUp.IgnoreAbilities.Add(m_Locomotion);

            // Half climb
            m_LowerClimb.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_LowerClimb.IgnoreAbilities.Add(m_Locomotion);
            m_LowerClimb.IgnoreAbilities.Add(m_Fall);
            m_LowerClimb.IgnoreAbilities.Add(m_Jump);

            // Climbing
            m_Climbing.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_Climbing.IgnoreAbilities.Add(m_Fall);
            m_Climbing.IgnoreAbilities.Add(m_Jump);

            // Drop to climb
            m_DropToClimb.IgnoreAbilities = new List<DiasGames.ThirdPersonSystem.ThirdPersonAbility>();
            m_DropToClimb.IgnoreAbilities.Add(m_Locomotion);

            // Vault
            m_Vault.IgnoreAbilities = new List<ThirdPersonAbility>();
            m_Vault.IgnoreAbilities.Add(m_Locomotion);
            m_Vault.IgnoreAbilities.Add(m_Jump);
            m_Vault.IgnoreAbilities.Add(m_Fall);
            m_Vault.IgnoreAbilities.Add(m_Crouch);

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
