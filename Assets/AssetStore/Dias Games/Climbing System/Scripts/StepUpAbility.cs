/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class StepUpAbility : ThirdPersonAbstractClimbing
    {
        public override bool TryEnterAbility()
        {
            if (!m_Engine.IsGrounded)
                return false;

            if (HasFoundLedge(out frontHit) && m_Engine.InputManager.RelativeInput.z >= -0.1f)
            {
                if (FreeAboveLedge())
                    return true;
            }

            return base.TryEnterAbility();
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_Engine.m_Capsule.enabled = false; // Deactivate collider
        }

        private void Reset()
        {
            m_EnterState = "Climb.Step Up";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseVerticalRootMotion = true;

            m_CastCapsuleRadius = 0.15f;
            m_VerticalLinecastStartPoint = 0.6f;
            m_VerticalLinecastEndPoint = 0.15f;
            m_MaxDistanceToFindLedge = 0.5f;
            m_VerticalDeltaFromLedge = 0.55f;
            m_ForwardDeltaFromLedge = 0.3f;
        }
    }
}