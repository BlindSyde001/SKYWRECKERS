/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class JumpAbility : ThirdPersonAbility
    {
        [Tooltip("Speed in y direction")] public float jumpPower = 6f; // The velocity that jump starts
        [Tooltip("Speed that player must have during a forward jumping")] public float speedOnJump = 7f; // The speed to move forward during a Jump
        [Tooltip("State of stationary jump")] [SerializeField] private string m_StationaryJumpState = "Air.JumpInPlace";
        

        public override string GetEnterState()
        {
            if (Mathf.Approximately(m_Engine.InputManager.RelativeInput.magnitude, 0) || m_UseRootMotion)
                return m_StationaryJumpState;

            return base.GetEnterState();
        }

        public override bool TryEnterAbility()
        {
            return (m_Engine.InputManager.jumpButton.WasPressed && m_Engine.IsGrounded);
        }

        public override void OnEnterAbility()
        {
            if (!Mathf.Approximately(m_Engine.InputManager.RelativeInput.magnitude, 0))
            {
                DoJump(jumpPower); // Make character jump, adding velocity in y direction

                transform.rotation = GetRotationFromDirection(m_Engine.InputManager.RelativeInput); // Rotate character to direction of input
                AddVelocityToJump(); // Add velocity to direction of Jump

                // Change State and parameters to allow jumping
                m_Engine.GroundCheckDistance = 0.01f;
                m_Engine.IsGrounded = false;
            }
            else
            {
                m_UseRootMotion = true;
                m_UseVerticalRootMotion = true;
            }


            base.OnEnterAbility();
        }

        public override bool TryExitAbility()
        {
            return m_Engine.IsGrounded && !m_UseRootMotion;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            
            m_UseRootMotion = false;
            m_UseVerticalRootMotion = false;
        }

        /// <summary>
        /// Add velocity to direction of Jump
        /// </summary>
        public void AddVelocityToJump()
        {
            Vector3 vel = transform.forward * speedOnJump; // Set velocity vector
            vel.y = m_Engine.m_Rigidbody.velocity.y; // Kepp vertical speed

            if (FreeOnMove(vel))
                m_Engine.m_Rigidbody.velocity = vel; // Set new velocity
        }
        

        /// <summary>
        /// Add force to character to make a jump
        /// </summary>
        /// <param name="power"></param>
        public void DoJump(float power)
        {
            Vector3 vel = m_Engine.m_Rigidbody.velocity;
            vel.y = power;
            m_Engine.m_Rigidbody.velocity = vel;
        }

        private void Reset()
        {
            m_EnterState = "Air.Jump";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = false;
        }
    }
}
