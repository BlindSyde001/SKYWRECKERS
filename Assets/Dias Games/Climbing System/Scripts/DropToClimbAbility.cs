/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class DropToClimbAbility : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_DroppableLayer; // Layers that character can drop bellow
        [SerializeField] private float m_DistanceToCast = 0.3f; // Distance in forward direction to start cast a ledge bellow
        [SerializeField] private float m_MaxHeightBellow = 0.1f; // Height bellow character position to cast

        [SerializeField] private float m_OffseFromLedge = 0.2f; // Distance to put character from ledge after find a ledge bellow

        [Tooltip("Set if player should drop automatically on a ledge or should press drop key")] [SerializeField] private bool m_AutomaticDrop = true;
        [Tooltip("If character move speed is greater than this value, character will not drop")] [SerializeField] private float m_MaxAllowedDropSpeed = 2f;

        private Vector3 m_PositionFromLedge; // Desired position after find ledge
        private Quaternion m_RotationOnLedge;

        public LayerMask DroppableLayer { get { return m_DroppableLayer; } set { m_DroppableLayer = value; } }
        public bool AutomaticDrop { get { return m_AutomaticDrop; } }

        /// <summary>
        /// Cast a ledge bellow character feet
        /// </summary>
        /// <param name="hitBellow"></param>
        /// <returns></returns>
        bool CastBellow(out RaycastHit hitBellow)
        {
            Vector3 start = transform.position + transform.forward * m_DistanceToCast + Vector3.down * m_MaxHeightBellow; // Start cast position

            if (Physics.Raycast(start, -transform.forward, out hitBellow, m_DistanceToCast, m_DroppableLayer)) // Try cast layer
            {
                if (Physics.CheckSphere(hitBellow.point + hitBellow.normal * 0.1f, 0.05f, Physics.AllLayers))
                    return false;

                if (hitBellow.normal.y < 0.1f) // Only drop if the ledge bellow is not a slope
                    return true;
            }

            return false;
        }

        public override bool TryEnterAbility()
        {
            if (m_Engine.m_Rigidbody.velocity.magnitude > m_MaxAllowedDropSpeed)
                return false;

            RaycastHit hit; // Hit information
            if (m_Engine.InputManager.dropButton.IsPressed || m_AutomaticDrop)
            {
                if (CastBellow(out hit)) // Check for a ledge bellow
                {
                    // Calculate position to start drop
                    m_PositionFromLedge = hit.point - hit.normal * m_OffseFromLedge;
                    m_PositionFromLedge.y = transform.position.y;

                    // Calculate rotation to drop
                    m_RotationOnLedge = Quaternion.FromToRotation(transform.forward, hit.normal);
                    m_RotationOnLedge.x = 0;
                    m_RotationOnLedge.z = 0;
                    transform.rotation *= m_RotationOnLedge;

                    return true;
                }
            }
            return base.TryEnterAbility();
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            StartCoroutine(SetPos());
            m_Engine.m_Capsule.enabled = false;
            m_Engine.m_Rigidbody.useGravity = false;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            m_Engine.m_Capsule.enabled = true;
            m_Engine.m_Rigidbody.useGravity = true;
        }


        private IEnumerator SetPos()
        {
            float step = Vector3.Distance(transform.position, m_PositionFromLedge) / 0.1f;
            float time = 0;

            while (time < 0.1f)
            {
                time += Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_PositionFromLedge, step * Time.fixedDeltaTime);
                yield return null;
            }

            transform.position = m_PositionFromLedge;
        }

        private void Reset()
        {
            m_EnterState = "Climb.Drop to Ledge Bellow";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;
        }

    }
}
