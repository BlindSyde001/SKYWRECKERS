using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class CrouchAbility : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_ObstacleMask = (1 << 0) | (1 << 14);
        [SerializeField] private float m_CapsuleScale = 0.6f;

        public override bool TryEnterAbility()
        {
            if (!IsFreeAbove())
                return true;

            if (m_Engine.IsGrounded && m_Engine.InputManager.crouchButton.IsPressed)
                return true;

            return false;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_Engine.ScaleCapsule(m_CapsuleScale);
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            m_Engine.RotateToDirection();

        }

        public override bool TryExitAbility()
        {
            return (!m_Engine.IsGrounded || !m_Engine.InputManager.crouchButton.IsPressed) && IsFreeAbove();
        }


        private bool IsFreeAbove()
        {
            float height = m_Engine.CapsuleOriginalHeight * m_CapsuleScale;
            float distance = m_Engine.CapsuleOriginalHeight - height;

            Vector3 start = transform.position + Vector3.up * (height - m_Engine.m_Capsule.radius - 0.1f);
            RaycastHit hit;
            if (Physics.SphereCast(start, m_Engine.m_Capsule.radius, Vector3.up, out hit, distance + 0.1f, m_ObstacleMask))
                return false;

            return true;
        }

        private void Reset()
        {
            m_EnterState = "Crouch";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_CustomCameraData = true;
        }
    }
}