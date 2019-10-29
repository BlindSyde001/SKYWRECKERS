/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class Locomotion : ThirdPersonAbility
    {
        [SerializeField] private bool m_UseStrafeMovement = false;        // Should always use strafe movement?
        [SerializeField] private string m_StrafeState = "Strafe";         // Strafe state used in the animator controller
        
        private string m_StrafeToUse;

        public bool UseStrafeMovement { get { return m_UseStrafeMovement; } }

        protected override void Awake()
        {
            base.Awake();
            m_StrafeToUse = m_StrafeState;
        }

        public override bool TryEnterAbility()
        {
            if (m_Engine.IsGrounded)
                return true;

            return false;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            
            bool useStrafe = m_Engine.IsZooming || m_UseStrafeMovement;
            
            if (useStrafe)
            {
                SetState(m_StrafeToUse);
            }
            else
            {
                SetState(m_EnterState);
            }

            m_Engine.RotateCharacter();
        }

        public override bool TryExitAbility()
        {
            return !m_Engine.IsGrounded;
        }
        
        private void Reset()
        {
            m_EnterState = "Grounded";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_CustomCameraData = true;
            m_AllowCameraZoom = true;
        }

        public void SetStrafeState(string strafeState)
        {
            if (strafeState == string.Empty)
                return;

            m_CurrentStatePlaying = null;
            m_StrafeToUse = strafeState;
        }

        public void ResetCustomStrafe()
        {
            m_CurrentStatePlaying = null;
            m_StrafeToUse = m_StrafeState;
        }

    }
}
