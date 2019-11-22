/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class LowerClimbAbility : ThirdPersonAbstractClimbing
    {
        public override bool TryEnterAbility()
        {
            if (HasFoundLedge(out frontHit))
            {
                if (FreeAboveLedge())
                {
                    if (m_Engine.IsGrounded)
                    {
                        if (m_Engine.InputManager.jumpButton.WasPressed)
                            return true;
                    }
                    else
                    {
                        return true;
                    }
                }
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
            m_EnterState = "Climb.Lower Climb";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseVerticalRootMotion = true;

            m_CastCapsuleRadius = 0.2f;
            m_VerticalLinecastStartPoint = 1.1f;
            m_VerticalLinecastEndPoint = 0.4f;
            m_MaxDistanceToFindLedge = 1f;
            m_VerticalDeltaFromLedge = 0.55f;
            m_ForwardDeltaFromLedge = 0.45f;
        }
    }
}
