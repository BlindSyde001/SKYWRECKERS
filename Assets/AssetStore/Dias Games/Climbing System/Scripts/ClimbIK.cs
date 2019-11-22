/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class ClimbIK : MonoBehaviour
    {
        // -------------------------------------------- MASTER CONTROL --------------------------------------------- //

        [Tooltip("Should it run hand ik when character is climbing?")] [SerializeField] private bool m_RunHandIK = true;
        [Tooltip("Should it run foot ik when character is climbing?")] [SerializeField] private bool m_RunFootIK = true;
        [Tooltip("Should it run body ik when character is hanging?")] [SerializeField] private bool m_RunBodyIK = true;

        [Tooltip("How far must cast to find ledge")] [SerializeField] private float m_DistanceToCast = 1f;

        private LayerMask m_HandLayers; // Which layer must cast
        private bool canRunIk = false; // Only run this IK during a climb
        public bool DebugCasts = true; // Should Debug the cast on Editor?

        // ------------------------------------------------------------------------------------------------------- //

        #region Hand
        // -------------------------------------------- HAND IK PARAMETERS ----------------------------------------------- //

        [Tooltip("Horizontal offset from surface to put hand")] [SerializeField] private float m_HandHorOffset = 0.05f;
        [Tooltip("Vertical offset from surface to put hand")] [SerializeField] private float m_HandVertOffset = -0.11f;
        [Tooltip("This offset brings hand to center.")] [SerializeField] private float m_HandCenterOffset = 0.1f;

        [Tooltip("Capsule cast radius for hands")] [SerializeField] private float m_HandCapsuleRadius = 0.01f;
        [Tooltip("Capsule cast height for hands")] [SerializeField] private float m_HandCapsuleHeight = 0.75f;

        [Tooltip("This is the distance between character pivot and hand position on the ledge when climbing")]
        [SerializeField] private float m_HandDistanceFromFeet = 1.5f;


        [Tooltip("How fast should hand start IK position. Lower values result in smoother positioning")]
        [SerializeField] private float m_HandIKSmooth = 0.01f;

        // Hands references position to start cast
        private Vector3 RightHandReference;
        private Vector3 LeftHandReference;

        private Vector3 rHandIKPos, lHandIKPos; // Desired hands IK position
        private Quaternion rHandIKRot, lHandIKRot; // Desired hands IK rotation

        // Internal vars to control IK behaviour
        private float handWeight = 0;

        #endregion

        #region Foot
        private Vector3 RightFootPosReference;
        private Vector3 LeftFootPosReference;

        private Vector3 rFootIKPos, lFootIKPos; // Final pos to put feet

        public LayerMask m_FeetLayers = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);
        [SerializeField] private float m_RightFootWallOfsset = 0.2f;
        [SerializeField] private float m_LeftFootWallOfsset = 0.2f;

        #endregion

        [SerializeField] private float m_BodyAdjusmentOnHang = 0.3f;

        private float bodyOffset = 0;
        private Vector3 m_BodyLocalPosition;
        private bool onHang = false;

        public bool OnHang { get { return onHang; } set { onHang = value; } }

        // Components
        private Animator m_Animator;
        private float rHandDelta, lHandDelta;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != 0)
                return;


            // -------------------------------- BODY IK ------------------------------ //

            bodyOffset = Mathf.Lerp(bodyOffset, (onHang && m_RunBodyIK) ? m_BodyAdjusmentOnHang : 0, 0.25f);
            if (m_RunBodyIK)
            {
                m_BodyLocalPosition = new Vector3(0, transform.InverseTransformPoint(m_Animator.bodyPosition).y, bodyOffset);
                m_Animator.bodyPosition = transform.TransformPoint(m_BodyLocalPosition);
            }

            // --------------------------------------------------------------------- //

            if (!canRunIk)
            {
                handWeight = Mathf.Lerp(handWeight, 0, 0.05f);
                return;
            }


            // -------------------------------- HAND IK ------------------------------ //

            if (m_RunHandIK)
            {
                handWeight = Mathf.Lerp(handWeight, 1, m_HandIKSmooth);

                // Set right hand ik weight
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handWeight);

                // Set left hand ik weight
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handWeight);

                // Set hands IK position and rotation
                SetIK(AvatarIKGoal.RightHand, rHandIKPos, rHandIKRot);
                SetIK(AvatarIKGoal.LeftHand, lHandIKPos, lHandIKRot);
            }

            // --------------------------------------------------------------------- //

            // --------------------------- FOOT IK --------------------------------- //

            if (m_RunFootIK && !onHang)
            {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

                SetFootIKPos(AvatarIKGoal.RightFoot, rFootIKPos);
                SetFootIKPos(AvatarIKGoal.LeftFoot, lFootIKPos);
            }

            // -------------------------------------------------------------------- //

            canRunIk = false;
        }




        /// <summary>
        /// It must be called by climbing component in every frame to calculate hand position (Think this method like an Update)
        /// </summary>
        /// <param name="topHit"></param>
        /// <param name="climbingMask"></param>
        /// <param name="CurrentLedge"></param>
        public void RunIK(RaycastHit topHit, LayerMask climbingMask, Transform CurrentLedge)
        {
            canRunIk = true;

            // ------------------------------------------------ HAND ------------------------------------------------------------- //

            SetHandReference(topHit, ref RightHandReference, HumanBodyBones.RightHand); // Set right hand start position for cast
            SetHandReference(topHit, ref LeftHandReference, HumanBodyBones.LeftHand); // Set left hand start position for cast

            m_HandLayers = climbingMask;

            // Start Casting
            CastHand(RightHandReference, ref rHandIKPos, ref rHandIKRot); // Cast for right hand
            CastHand(LeftHandReference, ref lHandIKPos, ref lHandIKRot); // Cast for left hand

            // ---------------------------------------------------------------------------------------------------------------- //



            // ------------------------------------------------ FOOT ------------------------------------------------------------- //

            SetFootReferenceStartPos(HumanBodyBones.RightFoot, ref RightFootPosReference);
            SetFootReferenceStartPos(HumanBodyBones.LeftFoot, ref LeftFootPosReference);

            CastFeet();

            // ---------------------------------------------------------------------------------------------------------------- //

        }


        #region Hand Methods

        /// <summary>
        /// Calculate the start point of cast a ledge for hands
        /// </summary>
        /// <param name="topHit"></param>
        /// <param name="handPosRef"></param>
        /// <param name="hand"></param>
        private void SetHandReference(RaycastHit topHit, ref Vector3 handPosRef, HumanBodyBones hand)
        {
            // Sets hand position to start raycasting
            handPosRef = m_Animator.GetBoneTransform(hand).transform.position;
            handPosRef.y = topHit.point.y; // Set y pos

            handPosRef = transform.InverseTransformPoint(handPosRef);
            handPosRef.z = 0; // Set to same local z pos of character
            handPosRef = transform.TransformPoint(handPosRef);
        }








        /// <summary>
        /// Cast ledge for a specific hand
        /// </summary>
        /// <param name="handReference"></param>
        /// <param name="handIKPos"></param>
        /// <param name="handIKRot"></param>
        /// <param name="handTransform"></param>
        void CastHand(Vector3 handReference, ref Vector3 handIKPos, ref Quaternion handIKRot)
        {
            RaycastHit handHit;

            float directionDelta = 1f;
            if (transform.InverseTransformPoint(handReference).x > 0)
                directionDelta = -1f;

            for (int i = 0; i < 20; i++)
            {
                // Set Capsule points to cast front ledge
                Vector3 cp1 = handReference + Vector3.down * m_HandCapsuleHeight * 0.5f + transform.right * 0.02f * directionDelta * i;
                Vector3 cp2 = cp1 + Vector3.up * m_HandCapsuleHeight;

                Debug.DrawLine(cp1, cp1 + transform.forward * m_DistanceToCast, Color.blue);
                Debug.DrawLine(cp2, cp2 + transform.forward * m_DistanceToCast, Color.blue);

                // Cast ledge from front
                if (Physics.CapsuleCast(cp1, cp2, m_HandCapsuleRadius, transform.forward, out handHit, m_DistanceToCast, m_HandLayers))
                {
                    // Sets new capsule points
                    Vector3 p1 = handHit.point + Vector3.up * (m_DistanceToCast) - transform.forward * m_HandCapsuleHeight * 0.5f;
                    Vector3 p2 = p1 + transform.forward * m_HandCapsuleHeight;

                    ////////////////////////////////////////////////////
                    // Debug on Editor both castings
                    if (DebugCasts)
                    {
                        Debug.DrawLine(p1, p1 + Vector3.down * m_DistanceToCast * 1.5f, Color.blue);
                        Debug.DrawLine(p2, p2 + Vector3.down * m_DistanceToCast * 1.5f, Color.blue);
                    }
                    ////////////////////////////////////////////////

                    // Cast from top and ignore hits not equals to front hit
                    RaycastHit[] topsHits = Physics.CapsuleCastAll(p1, p2, m_HandCapsuleRadius * 0.1f, Vector3.down, m_DistanceToCast * 1.5f, m_HandLayers);
                    foreach (RaycastHit topHit in topsHits)
                    {
                        if (topHit.transform == handHit.transform)
                        {
                            // Set hand ik pos and rot to be used in OnAnimtorIK
                            handIKPos = handHit.point - transform.forward * m_HandHorOffset;
                            handIKPos.y = topHit.point.y + m_HandVertOffset;

                            Quaternion rotFront = Quaternion.FromToRotation(Vector3.up, handHit.normal) * transform.rotation;
                            Quaternion rotTop = Quaternion.FromToRotation(transform.forward, topHit.normal) * transform.rotation;

                            handIKRot = new Quaternion(rotTop.x, rotFront.y, rotTop.z, rotFront.w);

                            if (directionDelta > 0)
                            {
                                lHandDelta = Mathf.Lerp(lHandDelta, 0.02f * i, 0.2f);
                            }
                            else
                            {
                                rHandDelta = Mathf.Lerp(rHandDelta, 0.02f * i, 0.2f);
                            }

                            return;
                        }
                    }
                }
            }

            handIKPos = Vector3.zero;
        }





        /// <summary>
        /// Set hand IK position and rotation to desired position found on casting
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handIKPos"></param>
        /// <param name="handIKRot"></param>
        /// <param name="animParam"></param>
        private void SetIK(AvatarIKGoal hand, Vector3 handIKPos, Quaternion handIKRot)
        {
            Vector3 desiredPos = m_Animator.GetIKPosition(hand); // Get hand current position

            desiredPos = transform.InverseTransformPoint(desiredPos); // Transform to local position
            desiredPos.x += (hand == AvatarIKGoal.RightHand) ? -(m_HandCenterOffset + rHandDelta) : m_HandCenterOffset + lHandDelta; // Put hand on center

            if (handIKPos != Vector3.zero) // If cast didn't fail
            {
                desiredPos.y += (handIKPos.y - transform.position.y) - m_HandDistanceFromFeet; // Keep y position fixed

                handIKPos = transform.InverseTransformPoint(handIKPos); // Transform hand ik pos to local position
                if (desiredPos.y < handIKPos.y)
                    desiredPos.y = handIKPos.y;

                //desiredPos.z = Mathf.Lerp(Mathf.Clamp(desiredPos.z, 0, handIKPos.z), handIKPos.z, m_Animator.GetFloat(animParam));
                desiredPos.z += bodyOffset;
                desiredPos.z = Mathf.Clamp(desiredPos.z, 0, handIKPos.z + bodyOffset);
                if (desiredPos.z > handIKPos.z + bodyOffset)
                    desiredPos.z = handIKPos.z + bodyOffset;

            }
            desiredPos = transform.TransformPoint(desiredPos);// Transform to world position

            // Set position and rotation of hand
            m_Animator.SetIKPosition(hand, desiredPos);
            m_Animator.SetIKRotation(hand, handIKRot);
        }

        #endregion

        #region Foot IK methods

        private void SetFootIKPos(AvatarIKGoal foot, Vector3 footIKPos)
        {
            Vector3 desiredPos = m_Animator.GetIKPosition(foot);

            if (footIKPos != Vector3.zero)
            {
                desiredPos = transform.InverseTransformPoint(desiredPos);
                footIKPos = transform.InverseTransformPoint(footIKPos);

                desiredPos.z += footIKPos.z;
                if (desiredPos.z > footIKPos.z)
                    desiredPos.z = footIKPos.z;

                desiredPos = transform.TransformPoint(desiredPos);
            }

            m_Animator.SetIKPosition(foot, desiredPos);
        }







        /// <summary>
        /// Cast forward to player to find wall and set feet position
        /// </summary>
        private void CastFeet()
        {
            RaycastHit footHit;
            Vector3 castDirection = transform.forward;
            castDirection.y = 0;

            if (Physics.SphereCast(RightFootPosReference, 0.1f, castDirection, out footHit, m_DistanceToCast, m_FeetLayers, QueryTriggerInteraction.Collide))
            {
                rFootIKPos = footHit.point + footHit.normal * m_RightFootWallOfsset;
            }
            else
                rFootIKPos = Vector3.zero;

            if (Physics.SphereCast(LeftFootPosReference, 0.1f, castDirection, out footHit, m_DistanceToCast, m_FeetLayers, QueryTriggerInteraction.Collide))
            {
                lFootIKPos = footHit.point + footHit.normal * m_LeftFootWallOfsset;
            }
            else
                lFootIKPos = Vector3.zero;
        }









        /// <summary>
        /// Set foot cast start position
        /// </summary>
        /// <param name="foot"></param>
        /// <param name="footReferencePos"></param>
        private void SetFootReferenceStartPos(HumanBodyBones foot, ref Vector3 footReferencePos)
        {
            footReferencePos = m_Animator.GetBoneTransform(foot).position;

            footReferencePos = transform.InverseTransformPoint(footReferencePos);
            footReferencePos.z = 0;
            footReferencePos = transform.TransformPoint(footReferencePos);
        }

        #endregion
    }
}
