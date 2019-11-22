/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DiasGames.ThirdPersonSystem
{
    public abstract class ThirdPersonAbility : MonoBehaviour
    {
        public bool Active { get; protected set; } = false; // Controls if this ability is active or not

        // ------------------------------------------- ANIMATIONS ----------------------------------------------------------------------------- //

        [Tooltip("Name of enter state of ability")] [SerializeField] protected string m_EnterState = "";
        [Tooltip("Animation transition duration to play this animation")] [SerializeField] protected float m_TransitionDuration = 0.1f;
        [Tooltip("Multiplier of root motion velocity in all axis")][SerializeField] protected Vector3 m_RootMotionMultiplier = Vector3.one;
        [Tooltip("Should exit this ability when animation ends?")] [SerializeField] protected bool m_FinishOnAnimationEnd = false;

        [Space(5)]

        // -----------------------------------------------------------------------------------------------------------------------------------//

        [Tooltip("Abilities that must be ignored to enter this ability. Means that this ability has higher priority than these ignored abilities")]
        public List<ThirdPersonAbility> IgnoreAbilities;



        [Tooltip("Should this ability uses root motion?")] [SerializeField] protected bool m_UseRootMotion = true;
        [Tooltip("Should this ability uses rotation root motion?")] [SerializeField] protected bool m_UseRotationRootMotion = false;
        [Tooltip("Should this ability uses root motion in vertical direction?")] [SerializeField] protected bool m_UseVerticalRootMotion = false;

        [SerializeField] protected bool m_AllowCameraZoom = false;                    // Allow camera zoom?
        [SerializeField] protected bool m_CustomCameraData = false;               // Should this ability uses its own camera data?
        [SerializeField] protected CameraData m_CameraData;                       // Custom camera data for this ability

        [SerializeField] protected PhysicMaterial m_AbilityPhysicMaterial;

        // ------------------------ PUBLIC GETTERS --------------------------------- //

        public bool UseRootMotion { get { return m_UseRootMotion; } }
        public bool UseRotationRootMotion { get { return m_UseRotationRootMotion; } }
        public bool UseVerticalRootMotion { get { return m_UseVerticalRootMotion; } }
        public Vector3 RootMotionMultiplier { get { return m_RootMotionMultiplier; } }

        public bool AllowCameraZoom { get { return m_AllowCameraZoom; } }
        public bool CustomCameraData { get { return m_CustomCameraData; } }
        public CameraData CameraData { get { return m_CameraData; } set { m_CameraData = value; } }

        public PhysicMaterial AbilityPhysicMaterial { get { return m_AbilityPhysicMaterial; } set { m_AbilityPhysicMaterial = value; } }

        // ----------------------------------------------------------------------- //


        // Vars
        protected ThirdPersonSystem m_Engine; // Character component reference
        protected AnimatorManager m_AnimatorManager; // Reference to animator controller

        public UnityEvent OnEnterAbilityEvent, OnExitAbilityEvent;

        protected string m_CurrentStatePlaying = ""; // Get current state playing
        public string CurrentStatePlaying { get { return m_CurrentStatePlaying; } }
        
        /// <summary>
        /// Constantly check to enter or leave ability
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // Check if it's active
            if (Active)
            {
                if(m_Engine.enabled == false)
                {
                    m_Engine.ExitAbility(this);
                    return;
                }

                // Check animation finish condition
                if (m_FinishOnAnimationEnd && m_AnimatorManager.HasFinishedAnimation(m_CurrentStatePlaying))
                {
                    m_Engine.ExitAbility(this); // Exit ability in the end of the animation
                    return;
                }

                // check conditions to exit ability
                if (TryExitAbility())
                    m_Engine.ExitAbility(this); // Exit ability from controller
            }
            else
            {
                // check condition to enter this ability
                if (TryEnterAbility())
                    m_Engine.OnTryEnterAbility(this); // enter ability
            }
        }

        /// <summary>
        /// Method to check conditions to enter ability
        /// </summary>
        /// <returns>True: enter ability</returns>
        public virtual bool TryEnterAbility() { return false; }

        /// <summary>
        /// Method called in the moment that ability is entered. Called once.
        /// </summary>
        public virtual void OnEnterAbility()
        {
            OnEnterAbilityEvent.Invoke();
            Active = true;
            SetState(GetEnterState());

            if (m_AbilityPhysicMaterial != null)
                m_Engine.m_Capsule.sharedMaterial = m_AbilityPhysicMaterial;
        }

        /// <summary>
        /// Fixed update for each ability. It's updated in the controller
        /// </summary>
        public virtual void FixedUpdateAbility()
        {
        }


        /// <summary>
        /// Method to check conditions to exit ability
        /// </summary>
        /// <returns>True: exit ability</returns>
        public virtual bool TryExitAbility() { return false; }


        /// <summary>
        /// Method called in the moment that ability exit. Called once.
        /// </summary>
        public virtual void OnExitAbility()
        {
            OnExitAbilityEvent.Invoke();
            Active = false;
            m_CurrentStatePlaying = string.Empty;
            m_Engine.ScaleCapsule(1f);
        }


        /// <summary>
        /// Method to set enter state. Called only in the TryEnterAbility() method
        /// Can be overwritten to start more than one animation
        /// </summary>
        /// <returns>Name of the state</returns>
        public virtual string GetEnterState() { return m_EnterState; }
            
        /// <summary>
        /// Sets a new state for this ability
        /// </summary>
        /// <param name="newState">Name of the new state</param>
        protected void SetState(string newState)
        {
            SetState(newState, m_TransitionDuration);
        }

        /// <summary>
        /// Sets a new state for this ability
        /// </summary>
        /// <param name="newState">Name of the new state</param>
        /// <param name="transitionDuration">Duration of transition</param>
        protected void SetState(string newState, float transitionDuration)
        {
            if (m_CurrentStatePlaying == newState)
                    return;

            m_AnimatorManager.SetAnimatorState(newState, transitionDuration, AnimatorManager.BaseLayerIndex);
            m_CurrentStatePlaying = newState; // Set new current state
        }

        protected virtual void Awake()
        {
            m_Engine = GetComponent<ThirdPersonSystem>();
            m_AnimatorManager = GetComponent<AnimatorManager>();
        }

        /// <summary>
        /// Get rotation to face desired direction
        /// </summary>
        /// <returns></returns>
        protected Quaternion GetRotationFromDirection(Vector3 direction)
        {
            float yaw = Mathf.Atan2(direction.x, direction.z);
            return Quaternion.Euler(0, yaw * Mathf.Rad2Deg, 0);
        }

        // Adds offset in Yaw rotation
        protected Quaternion GetRotationFromDirection(Vector3 direction, float yawOffset)
        {
            float yaw = Mathf.Atan2(direction.x, direction.z);
            return Quaternion.Euler(0, (yaw * Mathf.Rad2Deg) + yawOffset, 0);
        }

        // Check if character can walk on desired direction
        protected bool FreeOnMove()
        {
            Vector3 p1 = transform.position + Vector3.up * m_Engine.m_Capsule.radius * 2;
            Vector3 p2 = transform.position + Vector3.up * (m_Engine.m_Capsule.height - m_Engine.m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(p1, p2, m_Engine.m_Capsule.radius * 0.5f, transform.forward,
                                                        m_Engine.m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f, m_Engine.GroundMask, 
                                                        QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.normal.y < 0.5f && hit.collider.tag != "Player")
                    return false;
            }

            return true;
        }

        // Check if character can walk on desired direction
        protected bool FreeOnMove(Vector3 direction)
        {
            Vector3 p1 = transform.position + Vector3.up * m_Engine.m_Capsule.radius * 2;
            Vector3 p2 = transform.position + Vector3.up * (m_Engine.m_Capsule.height - m_Engine.m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(p1, p2, m_Engine.m_Capsule.radius * 0.5f, direction, 
                                                        m_Engine.m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f, m_Engine.GroundMask, 
                                                        QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.normal.y <= Mathf.Cos(m_Engine.MaxAngleSlope * Mathf.Deg2Rad) && hit.collider.tag != "Player")
                    return false;
            }

            return true;
        }

    }
}
