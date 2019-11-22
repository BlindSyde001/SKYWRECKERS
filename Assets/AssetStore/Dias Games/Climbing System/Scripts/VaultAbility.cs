using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class VaultAbility : ThirdPersonAbstractClimbing
    {
        [SerializeField] private float m_MaxVaultThickness = 0.3f;

        public override bool TryEnterAbility()
        {
            if (m_Engine.InputManager.jumpButton.WasPressed)
            {
                if (HasFoundLedge(out frontHit))
                {
                    if (IsVault())
                        return true;
                }
            }

            return base.TryEnterAbility();
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_Engine.m_Capsule.enabled = false;
        }

        private bool IsVault()
        {
            Vector3 freePoint = frontHit.point - frontHit.normal * (m_MaxVaultThickness + 0.1f);

             return !Physics.CheckSphere(freePoint, 0.09f, m_ClimbableMask);
        }

        private void Reset()
        {
            m_EnterState = "Climb.Vault";
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_RootMotionMultiplier = new Vector3(1f, 0.5f, 1f);

            m_VerticalDeltaFromLedge = 1f;
            m_ForwardDeltaFromLedge = 0.7f;
            m_PositioningSmoothnessTime = 0.1f;

            m_MaxDistanceToFindLedge = 2f;
            m_Iterations = 30;
            m_CastCapsuleRadius = 0.3f;

            m_VerticalLinecastStartPoint = 1.3f;
            m_VerticalLinecastEndPoint = 0.35f;
        }
    }
}