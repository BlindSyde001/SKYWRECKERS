/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class RollAbility : ThirdPersonAbility
    {
        [Tooltip("Speed during a roll")] public float speedOnRolling = 8f;
        [SerializeField] private float m_CapsuleScale = 0.6f;
        [SerializeField] private AudioClip rollClip;

        private AudioSource m_AudioSource;
        //private bool playedSound = false;

        protected override void Awake()
        {
            base.Awake();
            m_AudioSource = GetComponent<AudioSource>();
        }

        public override bool TryEnterAbility()
        {
            return (m_Engine.InputManager.rollButton.WasPressed && m_Engine.IsGrounded);
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            //playedSound = false;
            m_Engine.ScaleCapsule(m_CapsuleScale);

            if (!Mathf.Approximately(m_Engine.InputManager.RelativeInput.magnitude, 0))
                transform.rotation = GetRotationFromDirection(m_Engine.InputManager.RelativeInput); // Rotate character to direction of input
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            AddSpeedToRoll();

            /*if(m_Engine.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && !playedSound)
            {
                m_AudioSource.clip = rollClip;
                m_AudioSource.Play();
                playedSound = true;
            }*/
        }

        public void PlayRollSound()
        {
            if (rollClip == null)
                return;

            m_AudioSource.clip = rollClip;
            m_AudioSource.Play();
        }

        /// <summary>
        /// Move character to desired direction
        /// </summary>
        private void AddSpeedToRoll()
        {
            Vector3 vel = transform.forward * speedOnRolling;
            vel.y = m_Engine.m_Rigidbody.velocity.y;

            if (FreeOnMove(vel.normalized))
                m_Engine.m_Rigidbody.velocity = vel;
        }

        public override bool TryExitAbility()
        {
            return !m_Engine.IsGrounded;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            m_Engine.ScaleCapsule(1f);
            m_AudioSource.clip = null;
        }

        private void Reset()
        {
            m_EnterState = "Roll";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = false;
        }
    }
}
