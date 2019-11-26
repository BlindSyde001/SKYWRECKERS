/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DiasGames.ThirdPersonSystem.Cameras;

namespace DiasGames.ThirdPersonSystem
{
    public class ThirdPersonSystem : MonoBehaviour
    {
        #region Components

        // ------------------ Internal vars and components ----------------------- //

        private ThirdPersonAbility m_ActiveAbility = null; // Active ability
        private UnityInputManager m_InputManager;
        private CameraController m_CameraController;
        private AudioSource m_AudioSource;

        public ThirdPersonAbility ActiveAbility { get { return m_ActiveAbility; } }
        public CameraController CamController { get { return m_CameraController; } }

        /// <summary>Rigidbody component reference of charater </summary>
        public Rigidbody m_Rigidbody { get; set; }

        /// <summary>Aniamtor component reference of charater </summary>
        public Animator m_Animator { get; set; }

        /// <summary>Capsule collider component reference of charater </summary>
        public CapsuleCollider m_Capsule { get; set; }

        // ----------------------------------------------------------------------- //

        #endregion

        #region Exposed parameters
        /// <summary>
        /// Controls wheter the character is on ground or falling
        /// </summary>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// The normal vector of the ground. This information is useful to find angle of slope
        ///</summary>
        public Vector3 GroundNormal { get; private set; }

        [Tooltip("Layers to threat as a ground")] [SerializeField] private LayerMask m_GroundMask = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);

        [Tooltip("Distance from origin to find a ground. Higher values, earlier detects a ground")]
        [SerializeField] private float m_GroundCheckDistance = 0.35f;

        [Tooltip("Max slope angle, in degrees, that player can walk")]
        [SerializeField] private float m_MaxAngleSlope = 45f;

        [Tooltip("Should system apply extra gravity?")]
        [SerializeField] private bool m_ApplyExtraGravity = true;

        [Tooltip("Multiplier used to make jump more realistic")]
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;

        [Tooltip("Should character keeping zooming camera?")]
        [SerializeField] private bool m_AlwaysZoomCamera = false;
        
        public UnityEvent OnAnyAbilityEnters, OnAnyAbilityExits, OnGrounded;

        public List<Behaviour> m_BehavioursToDisable;

        #endregion

        #region Private internal parameters

        // Original parameters of capsule
        private float _capsuleOriginHeight;
        private Vector3 _capsuleOriginCenter;
        private PhysicMaterial capsuleOriginalMaterial;
        private RaycastHit m_GroundHit;

        // private vars
        private bool isZomming = false;
        private bool canZoomCamera = true;

        #endregion
        
        #region Public parameters and getter

        // ---------------------------------- GETTERS ------------------------------------- //

        public LayerMask GroundMask { get { return m_GroundMask; } set { m_GroundMask = value; } }

        public float MaxAngleSlope { get { return m_MaxAngleSlope; } } 

        public UnityInputManager InputManager { get { return m_InputManager; } }

        public bool IsZooming { get { return isZomming; } }

        public float CapsuleOriginalHeight { get { return _capsuleOriginHeight; } }

        // -------------------------------------------------------------------------------- //

        public float GroundCheckDistance { get; set; } // Created to allow change ground distance in runtime and preserve the initial m_GroundCheckDistance
        public bool IsCoroutinePlaying { get; set; } // Avoid play more than one coroutine per time

        #endregion

        #region Movement Parameters

        public float m_StationaryTurnSpeed = 180f;                      // Turn speed on idle state
        public float m_MovingTurnSpeed = 360f;                          // Turn speed when moving

        private float m_TurnAmount;                                     // Turn amount when moving
        private float m_ForwardAmount;                                  // forward amount when moving

        private float m_HorizontalAmount;                               // Horizontal amount for strafe
        private float m_VerticalAmount;                                 // Vertical amount for strafe

        private AnimatorManager m_AnimatorManager;                      // Reference to Animator Manager
        private Locomotion m_Locomotion;                                // Reference to Locomotion

        private bool m_UpdateMovement = true;
        public bool UpdateMovement { get { return m_UpdateMovement; } set { m_UpdateMovement = value; } }
        #endregion


        private bool m_IsAICharacter = false;
        public void SetControllerAsAI()
        {
            m_IsAICharacter = true;
        }


        private void Awake()
        {
            // Get components
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_InputManager = GetComponent<UnityInputManager>();
            m_CameraController = FindObjectOfType<CameraController>();
            m_AnimatorManager = GetComponent<AnimatorManager>();
            m_Locomotion = GetComponent<Locomotion>();
            m_AudioSource = GetComponent<AudioSource>();

            // Get initial dimensions of capsule
            _capsuleOriginCenter = m_Capsule.center;
            _capsuleOriginHeight = m_Capsule.height;
            capsuleOriginalMaterial = m_Capsule.sharedMaterial;

            //Set initial groundDistance
            GroundCheckDistance = m_GroundCheckDistance;

            IsCoroutinePlaying = false;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Freeze all rotations to allow scripts turn character
        }

        private void FixedUpdate()
        {
            CheckGround(); // check ground bellow character
            
            if (!IsGrounded)
                ExtraGravityOnAir();
            else
                HandleGrounded();
            
            if(m_UpdateMovement)
                UpdateMoveParameters();

            if (!m_IsAICharacter)
            {
                // Check Camera Zoom
                if (m_InputManager.zoomButton.IsPressed || m_AlwaysZoomCamera)
                    TryZoom();
                else
                    StopZoom();
            }

            // ----------------------- ABILITY UPDATE --------------------------- //

            if (m_ActiveAbility != null)
                m_ActiveAbility.FixedUpdateAbility();

            // ----------------------------------------------------------------- //
            
        }

        private void TryZoom()
        {
            if (ActiveAbility == null)
            {
                if(!isZomming)
                    m_CameraController.ZoomCamera();

                isZomming = true;
            }
            else
            {
                if (ActiveAbility.AllowCameraZoom)
                {
                    if (!isZomming)
                        m_CameraController.ZoomCamera();

                    isZomming = true;
                }
                else
                {
                    if (isZomming)
                        StopZoom();

                    isZomming = false;
                }
            }
        }

        private void StopZoom()
        {
            isZomming = false;
            m_CameraController.StopZoomCamera();

            if (ActiveAbility != null)
            {
                if (ActiveAbility.CustomCameraData)
                {
                    m_CameraController.SetCameraData(ActiveAbility.CameraData);
                    return;
                }
            }
            
            m_CameraController.SetCameraData();
        }

        #region Movement methods

        /// <summary>
        /// Update movement animator parameters
        /// </summary>
        private void UpdateMoveParameters()
        {
            if (!IsGrounded)
                return;

            Vector3 m_FreeMoveDirection = InputManager.RelativeInput;
            Vector3 m_StrafeDirection = InputManager.Move;

            if (m_StrafeDirection.magnitude > 1f)
                m_StrafeDirection.Normalize();

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (m_FreeMoveDirection.magnitude > 1f)
                m_FreeMoveDirection.Normalize();

            m_VerticalAmount = m_StrafeDirection.y;
            m_HorizontalAmount = m_StrafeDirection.x;

            m_FreeMoveDirection = transform.InverseTransformDirection(m_FreeMoveDirection);

            m_TurnAmount = Mathf.Atan2(m_FreeMoveDirection.x, m_FreeMoveDirection.z);

            if (FreeOnMove(InputManager.RelativeInput) || !IsGrounded)
                m_ForwardAmount = m_FreeMoveDirection.z;
            else
                m_ForwardAmount = 0;
            
            if (InputManager.walkButton.IsPressed)
                m_ForwardAmount = Mathf.Clamp(m_ForwardAmount, 0, 0.5f);

            // Set animator parameters of movement
            m_AnimatorManager.SetForwardParameter(m_ForwardAmount, 0.1f);
            m_AnimatorManager.SetTurnParameter(m_TurnAmount, 0.1f);
            m_AnimatorManager.SetVerticallParameter(m_VerticalAmount, 0.1f);
            m_AnimatorManager.SetHorizontalParameter(m_HorizontalAmount, 0.1f);
        }

        public void RotateCharacter()
        {
            if (isZomming || m_Locomotion.UseStrafeMovement)
                RotateByCamera();
            else
                RotateToDirection();
        }

        /// <summary>
        /// Rotates the character to direction of movement
        /// </summary>
        public void RotateToDirection()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        /// <summary>
        /// Rotate character to get same forward direction from camera
        /// </summary>
        public void RotateByCamera()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            transform.forward = Vector3.SmoothDamp(transform.forward, CamForward, ref velocity, 0.02f);
        }
        
        // Check if character can walk on desired direction
        private bool FreeOnMove(Vector3 direction)
        {
            Vector3 p1 = transform.position + Vector3.up * m_Capsule.radius * 2;
            Vector3 p2 = transform.position + Vector3.up * (m_Capsule.height - m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(p1, p2, m_Capsule.radius * 0.5f, direction,
                                                        m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f, GroundMask,
                                                        QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.normal.y <= Mathf.Cos(MaxAngleSlope * Mathf.Deg2Rad) && hit.collider.tag != "Player")
                    return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Method called by any ability to try enter ability
        /// </summary>
        /// <param name="ability"></param>
        public void OnTryEnterAbility(ThirdPersonAbility ability)
        {
            if (m_ActiveAbility == null)
                EnterAbility(ability);
            else
            {
                // Check if new ability has priority above current ability
                foreach (ThirdPersonAbility stopAbility in ability.IgnoreAbilities)
                {
                    if (stopAbility == m_ActiveAbility)
                        EnterAbility(ability);
                }
            }

        }

        /// <summary>
        /// Method that enter an ability. Can be also called to force any ability to enter
        /// </summary>
        /// <param name="ability"></param>
        public void EnterAbility(ThirdPersonAbility ability, bool forceAbility = false)
        {
            // Disable behaviours during a use of some behaviour
            foreach (Behaviour behaviour in m_BehavioursToDisable)
                behaviour.enabled = false;

            ExitActiveAbility();

            m_ActiveAbility = ability;

            m_ActiveAbility.OnEnterAbility();
            m_Animator.applyRootMotion = m_ActiveAbility.UseRootMotion;

            if (ActiveAbility.CustomCameraData)
                m_CameraController.SetCameraData(ActiveAbility.CameraData); // Set ability camera data

            OnAnyAbilityEnters.Invoke();
        }



        /// <summary>
        /// Method that exits an ability.
        /// </summary>
        /// <param name="ability"></param>
        public void ExitAbility(ThirdPersonAbility ability)
        {
            if(m_ActiveAbility == ability)
            {
                m_ActiveAbility = null;

                if (ability.Active)
                    ability.OnExitAbility();

                // Enable behaviours if no ability is active
                foreach (Behaviour behaviour in m_BehavioursToDisable)
                    behaviour.enabled = true;

                m_Capsule.sharedMaterial = capsuleOriginalMaterial;
                OnAnyAbilityExits.Invoke();
            }
        }

        /// <summary>
        /// Force current active ability to exit
        /// </summary>
        /// <param name="ability"></param>
        public void ExitActiveAbility()
        {
            if (m_ActiveAbility != null)
            {

                if (m_ActiveAbility.Active)
                    m_ActiveAbility.OnExitAbility();

                m_ActiveAbility = null;

                // Enable behaviours if no ability is active
                foreach (Behaviour behaviour in m_BehavioursToDisable)
                    behaviour.enabled = true;

                m_Capsule.sharedMaterial = capsuleOriginalMaterial;
                OnAnyAbilityExits.Invoke();
            }
        }


        // Called to apply root motion
        private void OnAnimatorMove()
        {
            // Vars that control root motion
            bool useRootMotion = false;
            bool verticalMotion = false;
            bool rotationMotion = false;
            Vector3 multiplier = Vector3.one;

            // Check if some ability is activated
            if (m_ActiveAbility != null)
            {
                useRootMotion = m_ActiveAbility.UseRootMotion;
                verticalMotion = m_ActiveAbility.UseVerticalRootMotion;
                rotationMotion = m_ActiveAbility.UseRotationRootMotion;
                multiplier = m_ActiveAbility.RootMotionMultiplier;
            }

            if (Mathf.Approximately(Time.deltaTime, 0f) || !useRootMotion) { return; } // Conditions to avoid animation root motion

            Vector3 delta = m_Animator.deltaPosition;

            delta = transform.InverseTransformVector(delta);
            delta = Vector3.Scale(delta, multiplier);

            delta = transform.TransformVector(delta);

            Vector3 vel =(delta) / Time.deltaTime; // Get animator movement
            
            if (!verticalMotion)
                vel.y = m_Rigidbody.velocity.y; // Preserve vertical velocity

            m_Rigidbody.velocity = vel; // Set character velocity

            if (rotationMotion)
                transform.rotation *= m_Animator.deltaRotation;

        }

        /// <summary>
        /// Execute an animation event called by animation
        /// </summary>
        /// <param name="eventName"></param>
        public void ExecuteAnimationEvent(string eventName)
        {
            GlobalEvents.ExecuteEvent(eventName, gameObject, null);
        }


        /// <summary>
        /// Checks ground bellow character
        /// </summary>
        private void CheckGround()
        {
            Debug.DrawRay(transform.position + (Vector3.up * 0.1f), Vector3.down * GroundCheckDistance, Color.white, 0.5f);
            //if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out m_GroundHit, GroundCheckDistance, m_GroundMask, QueryTriggerInteraction.Ignore))
            if (Physics.SphereCast(transform.position + (Vector3.up * 0.5f), 0.3f, Vector3.down, out m_GroundHit, GroundCheckDistance, m_GroundMask, QueryTriggerInteraction.Ignore))
                {
                if (m_GroundHit.normal.y > Mathf.Cos(m_MaxAngleSlope * Mathf.Deg2Rad)) // Calculate the angle of the ground. If it's higher than maxSlope, don't be grounded
                {

                    if (!IsGrounded)
                    {
                        OnGrounded.Invoke();
                    }

                    IsGrounded = true;
                    GroundNormal = m_GroundHit.normal;
                    return;
                }
            }
            

            IsGrounded = false;
            GroundNormal = Vector3.up;
        }
        


        /// <summary>
        /// Method to make gravity more realistic on Jump
        /// </summary>
        void ExtraGravityOnAir()
        {
            if (!m_Rigidbody.useGravity)
            {
                GroundCheckDistance = m_GroundCheckDistance;
                return;
            } // Don't apply force if gravity is not being applied.

            if (!m_ApplyExtraGravity)
                return;

            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce * m_Rigidbody.mass);

            GroundCheckDistance = m_Rigidbody.velocity.y < 2 ? m_GroundCheckDistance : 0.01f; // change ground distance to allow Jump
        }




        /// <summary>
        /// Method that restrict rigidbody velocity to avoid character goes up during a move
        /// </summary>
        void HandleGrounded()
        {
            if (!m_Rigidbody.useGravity) { return; } // Uses only with gravity applied and idle and walking states

            Vector3 vel = m_Rigidbody.velocity;
            vel.y = Mathf.Clamp(vel.y, -50, 0); // Avoid character goes up

            m_Rigidbody.velocity = vel;
        }


        /// <summary>
        /// Scale capsule collider
        /// </summary>
        /// <param name="scale">How much to scale (Uses initial dimension as reference) </param>
        public void ScaleCapsule(float scale)
        {
            m_Capsule.center = _capsuleOriginCenter * scale;
            m_Capsule.height = _capsuleOriginHeight * scale;
        }

    }
}
