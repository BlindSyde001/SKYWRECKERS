/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class ClimbLadderAbility : ThirdPersonAbility
    {
        [Tooltip("State to climb ladder on top")][SerializeField] private string m_LadderClimbUp = "Climb.Brace Climb Up";
        [Tooltip("State to jump above ladder")] [SerializeField] private string m_LadderJumpUp = "Climb.Hop Up";

        [Tooltip("Offset from forward face of ladder")][SerializeField] private float m_OffsetFromLadder = 0.35f;
        [Tooltip("Max distance to find a ladder")][SerializeField] private float m_MaxDistanceToCast = 1f;
        [Tooltip("Ladder mask")] [SerializeField] private LayerMask m_LadderMask = (1 << 20);
        [Tooltip("Positioning smothness on ladder")] [SerializeField] private float transitionTimeToPosition = 0.05f;

        [SerializeField] private Vector3 m_MultiplierOnJump = new Vector3(2f, 2f, 2f);
        
        public LayerMask LadderMask { get { return m_LadderMask; } }

        // Internal var
        private bool bDropped = false;
        RaycastHit ladderHit;

        public override bool TryEnterAbility()
        {
            if (m_Engine.IsGrounded)
                bDropped = false;

            if (m_Engine.InputManager.jumpButton.WasPressed || !m_Engine.IsGrounded)
            {
                if (FoundLadder(out ladderHit) && !bDropped)
                    return true;
            }

            return base.TryEnterAbility();
        }


        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            RaycastHit hit;
            if (FoundLadder(out hit))
            {
                ManageLadder(hit);
            }
            else
            {
                if (hit.point == Vector3.zero)
                { // There is no ladder bellow
                    float newForward = Mathf.Clamp(m_Engine.InputManager.Move.y, 0, 1f); // Avoid keep moving down
                    m_AnimatorManager.SetForwardParameter(newForward, 0.05f); // Stop moving down
                }
                else
                { // There is no ladder above
                    float newForward = Mathf.Clamp(m_Engine.InputManager.Move.y, -1f, 0f); // Avoid keep moving up
                    m_AnimatorManager.SetForwardParameter(newForward, 0.05f); // Stop moving up

                    if (FreeAbove())
                    { // If it's free above ladder

                        if (m_Engine.InputManager.Move.y > 0.2f && m_CurrentStatePlaying != m_LadderJumpUp)
                        {
                            // Climb ladder
                            SetState(m_LadderClimbUp);
                            m_Engine.m_Capsule.enabled = false;
                        }
                    }
                    else
                    { // It's not free above ladge

                        // Check to jump above
                        if(m_Engine.InputManager.Move.y > 0.2f)
                        {
                            if (m_Engine.InputManager.jumpButton.WasPressed) // Press to Jump
                            {
                                SetState(m_LadderJumpUp); // Jump up
                                m_RootMotionMultiplier = m_MultiplierOnJump;
                            }
                        }
                    }
                }
            }


            // --------------------------------------------- JUMP SIDE CONTROLLER ---------------------------------------------------- //

            if (Mathf.Abs(m_Engine.InputManager.Move.y) < 0.2f &&
                Mathf.Abs(m_Engine.InputManager.Move.x) > 0.2f)
            {
                if (m_Engine.InputManager.jumpButton.WasPressed)
                {
                    string state = (m_Engine.InputManager.Move.x > 0) ? "Climb.Jump Right" : "Climb.Jump Left";
                    SetState(state);
                    m_RootMotionMultiplier = m_MultiplierOnJump;
                }
            }

            // -------------------------------------------------------------------------------------------------------------------- //
        }

        public override bool TryExitAbility()
        {
            if(m_Engine.InputManager.dropButton.WasPressed)
            {
                bDropped = true;
                return true;
            }

            //Check if IsOnGround and pressed to down
            if (m_Engine.InputManager.Move.y < -0.2f && m_Engine.IsGrounded)
                return true;

            return base.TryExitAbility();
        }

        public override void OnExitAbility()
        {
            m_Engine.m_Rigidbody.useGravity = true;
            m_Engine.m_Capsule.enabled = true;
            m_RootMotionMultiplier = Vector3.one;
            base.OnExitAbility();
        }

        public override void OnEnterAbility()
        {
            m_Engine.m_Rigidbody.useGravity = false;
            StartCoroutine(SetCharacterPositionOnLadder(ladderHit));
            base.OnEnterAbility();
        }

        private void ManageLadder(RaycastHit hit)
        {
            // don't update during coroutine positioning
            if (m_Engine.IsCoroutinePlaying)
                return;

            if (m_CurrentStatePlaying == m_EnterState)
            {
                // Set character position and rotation all the time in ladder climbing
                Vector3 targetPosition = hit.point - hit.transform.forward * m_OffsetFromLadder;

                targetPosition = hit.transform.InverseTransformPoint(targetPosition);
                targetPosition.x = 0; // Set character to the center of ladder
                targetPosition = hit.transform.TransformPoint(targetPosition);

                transform.rotation = Quaternion.Euler(0, hit.transform.rotation.eulerAngles.y, 0);
                transform.position = targetPosition;
            }

            m_AnimatorManager.SetForwardParameter(m_Engine.InputManager.Move.y, 0.1f); // Stop moving down
            m_AnimatorManager.SetHorizontalParameter(m_Engine.InputManager.Move.x, 0.1f); // Stop moving down
            ///
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!Active || Mathf.Abs(m_Engine.InputManager.Move.y) > 0.2f)
                return;

            m_Engine.m_Animator.SetLookAtWeight(Mathf.Abs(m_Engine.InputManager.Move.x) * 0.5f);
            m_Engine.m_Animator.SetLookAtPosition(transform.position + Vector3.up * 1.5f + transform.right * m_Engine.InputManager.Move.x);
        }

        private bool FreeAbove()
        {
            Vector3 Start = transform.position + Vector3.up * (m_Engine.m_Capsule.height + 0.5f);
            RaycastHit hit;
            if(!Physics.SphereCast(Start, 0.25f, transform.forward, out hit, m_MaxDistanceToCast, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        private bool FoundLadder(out RaycastHit hitFromBottom)
        {
            RaycastHit hitTop; // Hit information from top

            Vector3 StartPointTop = transform.position + Vector3.up * (m_Engine.m_Capsule.height - m_Engine.m_Capsule.radius) - transform.forward * m_Engine.m_Capsule.radius;
            Vector3 StartPointBottom = transform.position - transform.forward * m_Engine.m_Capsule.radius;

            Vector3 direction = transform.forward;

            // Cast for top and bottom positions
            bool bFoundTop = Physics.Raycast(StartPointTop, direction, out hitTop, m_MaxDistanceToCast, m_LadderMask);
            bool bFoundBottom = Physics.Raycast(StartPointBottom, direction, out hitFromBottom, m_MaxDistanceToCast, m_LadderMask);

            Debug.DrawRay(StartPointBottom, direction * m_MaxDistanceToCast, Color.blue);
            Debug.DrawRay(StartPointTop, direction * m_MaxDistanceToCast, Color.blue);

            return (bFoundTop && bFoundBottom); // Return true only if both hits found ladder
        }


        /// <summary>
        /// Set character position with smooth
        /// </summary>
        /// <param name="bottomHit"></param>
        /// <returns></returns>
        private IEnumerator SetCharacterPositionOnLadder(RaycastHit bottomHit)
        {
            if (!m_Engine.IsCoroutinePlaying)
            {
                /// Parameters to cast right position on ladder///////////////////////////////////////////////////////////////

                RaycastHit hit; // Hit information for the new cast

                float distance = bottomHit.collider.bounds.max.magnitude; // Get max distance of the ladder collider

                Vector3 start = bottomHit.transform.position - bottomHit.transform.forward * (distance + 0.1f); // Cast start position
                start.y = transform.position.y; // Y position has to be the same of character

                Debug.DrawRay(start, bottomHit.transform.forward * 0.2f, Color.black, 5f); // Debug ray in the screen

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /// Cast ladder in its forward direction
                if (Physics.Raycast(start, bottomHit.transform.forward, out hit, distance + 0.2f, m_LadderMask))
                {
                    if (hit.transform == bottomHit.transform) // Only works if it's the same hit
                    {
                        m_Engine.IsCoroutinePlaying = true;

                        /// Founda ladder position to go
                        Quaternion targetRotation = Quaternion.Euler(0, bottomHit.transform.eulerAngles.y, 0); // Character should have the same rotation of ladder
                        Vector3 targetPosition = hit.point - hit.transform.forward * m_OffsetFromLadder; // Character desired rotation

                        /// Set character parameters ///////////////////
                        m_Engine.m_Capsule.enabled = false;
                        m_Engine.m_Rigidbody.useGravity = false;
                        m_Engine.m_Rigidbody.velocity = Vector3.zero;
                        ///////////////////////////////////////////////

                        float step = (Vector3.Distance(transform.position, targetPosition) / transitionTimeToPosition); // delta distance in each frame
                        float t = transitionTimeToPosition;

                        while (t > 0) // Update character position and rotation until target position
                        {
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step * Time.fixedDeltaTime);
                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
                            t -= Time.fixedDeltaTime;
                            yield return null;
                        }

                        transform.rotation = targetRotation;

                        m_Engine.m_Capsule.enabled = true;

                        m_Engine.IsCoroutinePlaying = false;
                    }
                }
            }
        }

        private void Reset()
        {
            m_EnterState = "Climb.Ladder";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;
        }
    }
}
