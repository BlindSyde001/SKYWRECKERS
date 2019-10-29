/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class ClimbingAbility : ThirdPersonAbstractClimbing
    {
        [Tooltip("Root motion multiplier for hop up")] [SerializeField] private Vector3 m_HopUpMotionMultiplier = new Vector3(1f, 1.75f, 1f);
        [Tooltip("Root motion multiplier on side jump")] [SerializeField] private Vector3 m_SideJumpMultiplier = new Vector3(2f, 2f, 2f);
        [Tooltip("Root motion multiplier for jump from wall")] [SerializeField] private Vector3 m_BackJumpMultiplier = new Vector3(2f, 3f, 2f);

        // --------------------------- STATES USED DURING CLIMBING ----------------------------- //

        [SerializeField] private string m_BraceFromTopStateName = "Climb.Brace From Top";
        [SerializeField] private string m_BeginHangStateName = "Climb.Begin Hang";
        [SerializeField] private string m_BracedHangStateName = "Climb.Braced";
        [SerializeField] private string m_HangStateName = "Climb.Hanging";
        [SerializeField] private string m_DropBracedStateName = "Climb.Drop from braced";
        [SerializeField] private string m_DropHangStateName = "Climb.Drop from Hang";
        [SerializeField] private string m_BracedClimbUpStateName = "Climb.Brace Climb Up";
        [SerializeField] private string m_HangClimbUpStateName = "Climb.Hang Climb Up";
        [SerializeField] private string m_HopUpStateName = "Climb.Hop Up";
        [SerializeField] private string m_JumpBackStateName = "Climb.Brace Jump Back";
        [SerializeField] private string m_BracedLookStateName = "Climb.LookToJump";
        [SerializeField] private string m_HangToBracedStateName = "Climb.Hang to Braced";
        [SerializeField] private string m_BracedToHangStateName = "Climb.Braced to Hang";

        [SerializeField] private string m_BracedTurnLeftStateName = "Climb.Braced Turn Left";
        [SerializeField] private string m_BracedTurnRightStateName = "Climb.Braced Turn Right";
        [SerializeField] private string m_HangTurnLeftStateName = "Climb.Hang Turn Left";
        [SerializeField] private string m_HangTurnRightStateName = "Climb.Hang Turn Right";

        // ------------------------------------------------------------------------------------ //


        // ---------------------------------------------------- SIDE CLIMBING PARAMETERS ------------------------------------------------ //

        [Tooltip("Value must be less than Side Distance From Char Origin to cast works fine")] [SerializeField] private float m_SideCapsuleRadius = 0.1f;
        [SerializeField] private float m_SideCapsuleHeight = 0.75f;
        [Tooltip("Max Distance from character position to check a ledge")] [SerializeField] private float m_SideMaxDistanceToCast = 1f;

        [Tooltip("Distance from character position that should start cast in horizontal direction")]
        [SerializeField] private float m_SideDistanceFromCharacter = 0.7f;

        [Tooltip("Distance from ledge limit to set new position for character")] [SerializeField] private float m_CharacterOffsetOnSide = 0.5f;

        [Space(15)]
        [Tooltip("Time in seconds during a turning in a ledge")] [SerializeField] private float m_TimeToTurnLedge = 2.2f;

        private bool m_SideAdjustment = false;

        // -------------------------------------------------------------------------------------------------------------------------- //

        private string startState = string.Empty;
        private bool bWallOnFoot = false; // Is there wall in front of feet?
        
        // Components
        private ClimbIK m_ClimbIK; // Controls climbing IK

        // --------------------- FAST MOVE ON CLIMBING -------------------------- //

        private float m_MoveMultiplier = 1f;
        private float m_MultiplierLastTime = 0;

        // --------------------------------------------------------------------- //


        // ---------- CONTROLLER OF POSITION WHEN CHARACTER IS CLIMBING A MOVABLE LEDGE --------- //

        private Vector3 m_LedgeLastPosition = Vector3.zero;
        private Transform m_LastLedgeTransform = null;
        private float m_LastTime = 0;

        // ------------------------------------------------------------------------------------- //

        private float enterAbilityTime = 0;

        protected override void Awake()
        {
            base.Awake();
            m_ClimbIK = GetComponent<ClimbIK>();
        }

        public override string GetEnterState()
        {
            bool IsPlayerAboveLedge = (transform.position.y + m_VerticalDeltaFromLedge) > topHit.point.y;

            if (bWallOnFoot)
            {
                if (IsPlayerAboveLedge)
                    return m_BraceFromTopStateName;
            }
            else
                return m_BeginHangStateName;

            return base.GetEnterState();
        }

        public override bool TryEnterAbility()
        {
            if (m_Engine.IsGrounded)
                return false;

            if (HasFoundLedge(out frontHit, false))
            {
                WallOnFeet();
                return true;
            }

            return base.TryEnterAbility();
        }

        public override void OnEnterAbility()
        {
            enterAbilityTime = Time.fixedTime;
            m_FinishOnAnimationEnd = false;
            m_RootMotionMultiplier = Vector3.one;
            startState = GetEnterState();

            if (m_ClimbIK != null)
                m_ClimbIK.OnHang = !bWallOnFoot;


            bool right = CanClimbSide(1, false);
            bool left = CanClimbSide(-1, false);

            if (!right && left)
                frontHit.point -= hitReference.right * m_CharacterOffsetOnSide * 0.5f;
            if (right && !left)
                frontHit.point += hitReference.right * m_CharacterOffsetOnSide * 0.5f;

            base.OnEnterAbility();
        }


        public override void OnExitAbility()
        {
            m_FinishOnAnimationEnd = false;

            if (m_ClimbIK != null)
                m_ClimbIK.OnHang = false;

            base.OnExitAbility();
        }

        /// <summary>
        /// Check if exists wall in front of feets
        /// </summary>
        /// <returns></returns>
        private void WallOnFeet()
        {
            Vector3 Start = transform.position - transform.forward * m_CastCapsuleRadius * 2; // Set Start position to cast

            RaycastHit wallHit;
            Vector3 direction = (frontHit.point - transform.position).normalized; // Get direction to cast
            direction.y = 0;

            // Cast both feet
            bool rightWall = Physics.SphereCast(Start + Vector3.right * 0.25f, m_CastCapsuleRadius, direction, out wallHit, m_MaxDistanceToFindLedge + m_CastCapsuleRadius * 2 + 0.5f, m_Engine.GroundMask);
            bool leftWall = Physics.SphereCast(Start + Vector3.left * 0.25f, m_CastCapsuleRadius, direction, out wallHit, m_MaxDistanceToFindLedge + m_CastCapsuleRadius * 2 + 0.5f, m_Engine.GroundMask);

            // Only set bWallOnFoot bool if both feet have the same value
            if (rightWall && leftWall)
                bWallOnFoot = true;

            if (!rightWall && !leftWall)
                bWallOnFoot = false;
        }


        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            // Check if it finishes start animation
            if (m_AnimatorManager.HasFinishedAnimation(startState) && !m_FinishOnAnimationEnd && m_CurrentStatePlaying == startState)
            {
                string state = (bWallOnFoot) ? m_BracedHangStateName : m_HangStateName;
                SetState(state, m_TransitionDuration);
            }

            CheckToTurn();

            // constantly update found ledge
            if (HasFoundLedge(out frontHit, true))
            {
                WallOnFeet();

                if (!m_FinishOnAnimationEnd)
                {
                    // ------------------------------ CHANGE BETWEEN HANG AND BRACED CLIMBING -------------------------------- //

                    if (bWallOnFoot)
                    {
                        if (m_CurrentStatePlaying == m_HangStateName)
                            StartCoroutine(ChangeClimbType(ClimbType.Braced));
                    }
                    else
                    {
                        if (m_CurrentStatePlaying == m_BracedHangStateName)
                            StartCoroutine(ChangeClimbType(ClimbType.Hang));
                    }

                    // ----------------------------------------------------------------------------------------------------- //

                    m_CurrentLedgeTransform = topHit.transform; // Set current ledge as the ledge that character is holding
                    if (!m_Engine.IsCoroutinePlaying)
                    {
                        SetCharacterPositionOnLedge();
                        if (m_ClimbIK != null)
                            m_ClimbIK.RunIK(topHit, m_ClimbableMask, m_CurrentLedgeTransform);
                    }
                    
                }
                else
                {
                    if (m_CurrentStatePlaying == m_HopUpStateName)
                    {
                        if (m_CurrentLedgeTransform != topHit.transform)
                        {
                            OnEnterAbility();
                        }
                    }
                }
            }


            // ------------------ Ledge position delta -------------------- //

            if (m_CurrentLedgeTransform != null)
            {
                Vector3 ledgeDeltaPosition = Vector3.zero;

                if (m_LastTime != 0 && (Time.fixedTime - m_LastTime) < Time.fixedDeltaTime * 5f && m_LastLedgeTransform == m_CurrentLedgeTransform)
                    ledgeDeltaPosition = m_CurrentLedgeTransform.position - m_LedgeLastPosition;

                transform.position += ledgeDeltaPosition;

                m_LastLedgeTransform = m_CurrentLedgeTransform;
                m_LedgeLastPosition = m_CurrentLedgeTransform.position;
                m_LastTime = Time.fixedTime;
            }

            // ------------------------------------------------------------ //

            InputControl();

            // Set horizontal and vertical parameters

            if (!m_SideAdjustment)
            {
                float horizontal = m_Engine.InputManager.Move.x;
                if (CanClimbSide(m_Engine.InputManager.Move.x, true))
                {
                    if (m_CurrentStatePlaying == m_BracedLookStateName)
                        SetState(m_BracedHangStateName);
                }
                else
                {
                    if (m_CurrentStatePlaying == m_BracedHangStateName)
                        SetState(m_BracedLookStateName);

                    if (!bWallOnFoot) // Check if it's hanging to stop movement
                        horizontal = 0;
                }
                m_AnimatorManager.SetHorizontalParameter(horizontal * m_MoveMultiplier, 0.1f);
            }

            m_AnimatorManager.SetForwardParameter(m_Engine.InputManager.Move.y);

        }

        private void InputControl()
        {
            // Fast move
            if (m_Engine.InputManager.jumpButton.WasPressed)
            {
                m_MoveMultiplier = 2f;
                m_MultiplierLastTime = Time.fixedTime + 0.5f;
            }

            if (m_MultiplierLastTime <= Time.fixedTime)
                m_MoveMultiplier = 1f;

            if (m_CurrentStatePlaying == startState || m_FinishOnAnimationEnd || m_Engine.IsCoroutinePlaying)
                return;

            // Drop condition
            if (m_Engine.InputManager.dropButton.WasPressed)
            {
                string state = (bWallOnFoot) ? m_DropBracedStateName : m_DropHangStateName;
                SetState(state);
                m_FinishOnAnimationEnd = true;
            }

            //Climb up
            if (m_Engine.InputManager.Move.y > 0.5f && Mathf.Abs(m_Engine.InputManager.Move.x) < 0.3f)
            {
                if (FreeAboveLedge())
                {
                    // -------------- CLIMB UP --------------- //

                    string state = (bWallOnFoot) ? m_BracedClimbUpStateName : m_HangClimbUpStateName;
                    m_FinishOnAnimationEnd = true;
                    m_Engine.m_Capsule.enabled = false;
                    SetState(state);
                    return;

                    // ------------------------------------- //
                }
                else
                {
                    if (m_Engine.InputManager.jumpButton.WasPressed && bWallOnFoot)
                    {
                        // --------------- HOP UP ---------------- //

                        m_FinishOnAnimationEnd = true;
                        m_RootMotionMultiplier = m_HopUpMotionMultiplier;
                        SetState(m_HopUpStateName);

                        // ------------------------------------- //
                    }
                }
            }

            if (!bWallOnFoot)
                return;

            // ------------------------------------------------- SIDE JUMP --------------------------------------------------------------------------- //

            if (Mathf.Abs(m_Engine.InputManager.Move.x) > 0.2f && Mathf.Abs(m_Engine.InputManager.Move.y) < 0.5f)
            {
                if (m_Engine.InputManager.jumpButton.WasPressed)
                {
                    if (m_CurrentStatePlaying == m_BracedLookStateName)
                    {
                        m_FinishOnAnimationEnd = true;
                        m_RootMotionMultiplier = m_SideJumpMultiplier;
                        string state = (m_Engine.InputManager.Move.x > 0) ? "Climb.Jump Right" : "Climb.Jump Left";
                        SetState(state);
                    }
                }
            }

            // ------------------------------------------------------------------------------------------------------------------------------------- //



            // ------------------------------------------------- BACK JUMP --------------------------------------------------------------------------- //

            if (m_Engine.InputManager.Move.y < -0.5f && Mathf.Abs(m_Engine.InputManager.Move.x)<0.2f)
            {
                if (m_Engine.InputManager.jumpButton.WasPressed)
                {
                    m_FinishOnAnimationEnd = true;
                    m_RootMotionMultiplier = m_BackJumpMultiplier;
                    SetState(m_JumpBackStateName);
                }
            }

            // ------------------------------------------------------------------------------------------------------------------------------------- /
        }

        /// <summary>
        /// This method cast ledge on side to check if character can shimmy or not. If character get the limit of a ledge, it automatcally fix character position
        /// </summary>
        /// <param name="hordirection"></param>
        /// <param name="canTurn"></param>
        /// <returns></returns>
        private bool CanClimbSide(float hordirection, bool canTurn)
        {
            if (Mathf.Approximately(hordirection, 0))
                return true;

            Vector3 HandReference = transform.position + Vector3.up * 1.5f;
            float direction = (hordirection > 0) ? 1 : -1;

            ////////////////////////////////////////////////////////////////////////////////////////////
            // Set capsule points to cast
            ////////////////////////////////////////////////////////////////////////////////////////////

            Vector3 cp1 = transform.position + transform.right * direction * m_SideDistanceFromCharacter;

            if (!canTurn)
            {
                GameObject getRotationGameObject = new GameObject();
                getRotationGameObject.transform.position = topHit.point;
                getRotationGameObject.transform.forward = -frontHit.normal;
                cp1 = topHit.point + getRotationGameObject.transform.right * direction * m_SideDistanceFromCharacter + frontHit.normal * m_SideMaxDistanceToCast * 0.5f;
                

                Destroy(getRotationGameObject.gameObject);
            }

            cp1.y = HandReference.y - m_SideCapsuleHeight * 0.5f;

            Vector3 cp2 = cp1 + Vector3.up * m_SideCapsuleHeight;

            ////////////////////////////////////////////////////////////////////////////////////////////



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // First cast: Cast on side, in direction of movement. If find a ledge, return true to allow character keeping moving
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            RaycastHit hit;
            if (Physics.CapsuleCast(cp1, cp2, m_SideCapsuleRadius, -frontHit.normal, out hit, m_SideMaxDistanceToCast, ClimbingMask | (1 << 21)))
            {
                if (hit.transform == topHit.transform)
                    return true;
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Second cast: this cast check the ledge limit and set character position on the limit
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            cp1 += transform.forward * HorizontalDeltaFromLedge;
            cp2 += transform.forward * HorizontalDeltaFromLedge;

            Vector3 newPos = Vector3.zero;
            RaycastHit[] hitsFromSide = Physics.CapsuleCastAll(cp1, cp2, m_SideCapsuleRadius, -transform.right * direction, m_SideDistanceFromCharacter, ClimbingMask);
            foreach (RaycastHit hitSide in hitsFromSide)
            {
                if (hitSide.transform == topHit.transform)
                {
                    if (hitSide.point == Vector3.zero) { return false; }

                    newPos = transform.InverseTransformPoint(hitSide.point);

                    newPos.z = 0;
                    newPos.y = 0;
                    newPos.x -= m_CharacterOffsetOnSide * direction;

                    newPos = transform.TransformPoint(newPos);

                    break;
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Third cast: this cast check condition to character turn a ledge only if ledge is big enough to allow this. 
            // First, it overlaps the region to not allow turn if a obstacle is in there.
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (canTurn)
            {
                Vector3 cp3 = cp1 + transform.forward * (m_SideCapsuleRadius + m_Engine.m_Capsule.radius);
                Vector3 cp4 = cp2 + transform.forward * (m_SideCapsuleRadius + m_Engine.m_Capsule.radius);

                if (!Physics.CheckCapsule(cp3, cp4, m_SideCapsuleRadius, Physics.AllLayers, QueryTriggerInteraction.Collide))
                {
                    if (Physics.CapsuleCast(cp3, cp4, m_SideCapsuleRadius, -transform.right * direction, out hit, m_SideMaxDistanceToCast, ClimbingMask))
                    {
                        if (hit.transform == topHit.transform)
                        {
                            float multiplier = (m_CurrentStatePlaying == m_BracedHangStateName) ? 1f : 2f;
                            StartCoroutine(TurnCharacterOnLedge(direction, Time.fixedTime + m_TimeToTurnLedge * multiplier));
                            return true;
                        }
                    }
                }
            }

            if (newPos != Vector3.zero && (m_CurrentStatePlaying == m_BracedHangStateName))
                StartCoroutine(FixCharacterPosition(newPos));

            return false;
        }




        /// <summary>
        ///  Enumarator called to turn a character in a corner
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="timeToFinish"></param>
        /// <returns></returns>
        IEnumerator TurnCharacterOnLedge(float direction, float timeToFinish)
        {
            if (!m_SideAdjustment && !m_Engine.IsCoroutinePlaying)
            {
                m_SideAdjustment = true;

                while (timeToFinish > Time.fixedTime)
                {
                    m_AnimatorManager.SetHorizontalParameter(direction);
                    yield return null;
                }

                m_SideAdjustment = false;
            }
        }



        /// <summary>
        /// This fix character position in the end of ledge
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerator FixCharacterPosition(Vector3 target)
        {
            if (!m_FinishOnAnimationEnd)
            {
                float speed = Vector3.Distance(target, transform.position) / 0.05f;
                float time = 0;

                while (time < 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
                    time += Time.fixedDeltaTime;
                    yield return null;
                }
            }

        }



        private enum ClimbType { Braced, Hang}
        /// <summary>
        /// Change between braced and hang climb
        /// </summary>
        /// <param name="m_ClimbType"></param>
        /// <returns></returns>
        IEnumerator ChangeClimbType(ClimbType m_ClimbType)
        {
            if (!m_Engine.IsCoroutinePlaying && !m_SideAdjustment)
            {
                m_Engine.IsCoroutinePlaying = true;

                string state = (m_ClimbType == ClimbType.Braced) ? m_HangToBracedStateName : m_BracedToHangStateName;

                if (m_ClimbType == ClimbType.Braced)
                {
                    if (m_ClimbIK != null)
                        m_ClimbIK.OnHang = false;
                }

                SetState(state);
                
                while (m_CurrentStatePlaying != state)
                    yield return null;

                while (!m_AnimatorManager.HasFinishedAnimation(state))
                    yield return null;

                state = (m_ClimbType == ClimbType.Braced) ? m_BracedHangStateName : m_HangStateName;

                if (m_ClimbType == ClimbType.Hang)
                {
                    if (m_ClimbIK != null)
                        m_ClimbIK.OnHang = true;
                }

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                SetState(state, 0.05f);
                m_Engine.IsCoroutinePlaying = false;

            }
        }

        private void CheckToTurn()
        {
            if (Time.fixedTime - enterAbilityTime < 1f)
                return;

            Vector3 capsulePoint1 = transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius);
            Vector3 capsulePoint2 = transform.position + Vector3.up * (m_VerticalLinecastEndPoint + m_CastCapsuleRadius);

            float castDirection = m_Engine.InputManager.Move.x;
            RaycastHit sideHit;
            if(Physics.CapsuleCast(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, transform.right * castDirection, out sideHit, 0.6f, m_ClimbableMask))
            {
                Vector3 directionToCheckLedge = (sideHit.point - transform.position); // Get direction from player to point that found ledge
                directionToCheckLedge.y = 0;

                float step = 0.6f / m_Iterations;

                for (int i = 0; i < m_Iterations; i++)
                {
                    float endCast = m_VerticalLinecastEndPoint;

                    if (m_UpdateCastByVerticalSpeed)
                    {
                        // Adjust linecast size according velocity of jump
                        endCast = m_VerticalLinecastEndPoint + m_Engine.m_Rigidbody.velocity.y / 10f;
                        endCast = Mathf.Clamp(endCast, 0, m_VerticalLinecastEndPoint);
                    }

                    //Sets start point and endpoint of linecast
                    Vector3 Start = transform.position + Vector3.up * m_VerticalLinecastStartPoint + directionToCheckLedge.normalized * step * i;
                    Vector3 End = transform.position + Vector3.up * endCast + directionToCheckLedge.normalized * step * i;

                    if(Physics.Raycast(Start, Vector3.down, Start.y - End.y, m_ClimbableMask))
                    {
                        hitReference.forward = sideHit.normal;
                        float angleDelta = transform.rotation.eulerAngles.y - hitReference.eulerAngles.y;

                        string state = (bWallOnFoot) ? 
                            ((castDirection > 0) ? m_BracedTurnRightStateName : m_BracedTurnLeftStateName)
                            : ((castDirection > 0) ? m_HangTurnRightStateName : m_HangTurnLeftStateName);

                        StartCoroutine(InternalTurn(angleDelta, state));
                    }
                }
            }
        }

        IEnumerator InternalTurn(float angleDelta, string state)
        {
            if (!m_Engine.IsCoroutinePlaying)
            {
                m_Engine.IsCoroutinePlaying = true;

                SetState(state);

                while (!m_Engine.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).IsName(state))
                    yield return null;

                float clipDuration = m_Engine.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).length;

                Quaternion desiredRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y + angleDelta, 0);

                while(m_Engine.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).normalizedTime < 0.9f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, 0.05f / clipDuration);
                    yield return null;
                }

                SetState(state.Contains("Braced") ? m_BracedHangStateName : m_HangStateName);
                m_Engine.IsCoroutinePlaying = false;
            }
        }

        private void Reset()
        {
            m_ClimbableMask = (1 << 18) | (1 << 19);
            m_EnterState = "Climb.Brace From Down";
            m_TransitionDuration = 0.2f;
            m_Iterations = 25;
            m_UpdateCastByVerticalSpeed = true;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;

            m_CastCapsuleRadius = 0.1f;
            m_VerticalLinecastStartPoint = 1.8f;
            m_VerticalLinecastEndPoint = 1.1f;
            m_MaxDistanceToFindLedge = 1f;
            m_VerticalDeltaFromLedge = 1.5f;
            m_ForwardDeltaFromLedge = 0.3f;
            m_PositioningSmoothnessTime = 0.1f;

            m_CustomCameraData = true;
        }

    }
}
