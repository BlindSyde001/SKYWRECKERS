/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public abstract class ThirdPersonAbstractClimbing : ThirdPersonAbility
    {
        [Tooltip("Layers that player can cast ledge and climb")] [SerializeField] protected LayerMask m_ClimbableMask;

        [Tooltip("Capsule radius to be used in the Capsule Cast")] [SerializeField] protected float m_CastCapsuleRadius = 0.3f;
        [Tooltip("How many iteration engines should do after find a ledge")] [SerializeField] protected int m_Iterations = 10;

        [SerializeField] protected float m_VerticalLinecastStartPoint = 2f;
        [SerializeField] protected float m_VerticalLinecastEndPoint = 1.5f;
        [Tooltip("If true, updates linecast range by velocity in y axis")] public bool m_UpdateCastByVerticalSpeed = false;
        [Tooltip("Max distance to cast a ledge from current position")] [SerializeField] protected float m_MaxDistanceToFindLedge = 1f;


        [Tooltip("Distance from ledge surface to set character y pos before start climbing")] [SerializeField] protected float m_VerticalDeltaFromLedge = 1f;
        [Tooltip("Distance from ledge to set character horizontal position before start climbing")] [SerializeField] protected float m_ForwardDeltaFromLedge = 0.3f;
        [Tooltip("Time to set character position on ledge")] [SerializeField] protected float m_PositioningSmoothnessTime = 0.1f;

        [SerializeField] private Color gizmoColor = Color.red; // The color that editor must draw on Scene

        protected RaycastHit frontHit, topHit;
        protected Transform m_CurrentLedgeTransform;
        protected Transform hitReference;

        /// //////////////////////////////////////////////////////////////////////////////////
        /// Public getters for parameters of climbing
        /// //////////////////////////////////////////////////////////////////////////////////

        //public Transform LastLedgeTransform { get { return lastLedgeTransform; } }
        public Transform CurrentLedgeTransform { get { return m_CurrentLedgeTransform; } }
        public LayerMask ClimbingMask { get { return m_ClimbableMask; } set { m_ClimbableMask = value; } }
        public float HorizontalDeltaFromLedge { get { return m_ForwardDeltaFromLedge; } }

        /// /////////////////////////////////////////////////////////////////////////////////

        private LayerMask m_LadderMask = (1<<20);

        protected override void Awake()
        {
            base.Awake();
            ClimbLadderAbility climbLadderAbility = GetComponent<ClimbLadderAbility>();
            if (climbLadderAbility != null)
                m_LadderMask = climbLadderAbility.LadderMask;
        }

        public override void OnEnterAbility()
        {
            m_Engine.m_Rigidbody.isKinematic = true; // Stop movement from rigidbody
            m_Engine.m_Rigidbody.useGravity = false; // Stop gravity applying during a climb
            m_UpdatePosition = true; // Update character position to fit ledge

            base.OnEnterAbility();
        }


        public override void OnExitAbility()
        {
            m_Engine.m_Capsule.enabled = true; 
            m_Engine.m_Rigidbody.useGravity = true;
            base.OnExitAbility();
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            UpdateCharPosition();
        }

        /// <summary>
        /// First step of casting. It casts a Capsule in the direction of character movement trying to find a ledge around
        /// </summary>
        /// <param name="capsuleHit"></param>
        /// <returns></returns>
        protected bool HasFoundLedge(out RaycastHit capsuleHit, bool m_IsAlreadyClimbing = true)
        {
            ////////////////////////////////////////
            // Capsule Cast is used to check an existing ledge in front of player
            ///////////////////////////////////////

            capsuleHit = new RaycastHit();

            //if (character.CharState == CharacterState.Blocked) { return false; }

            float endCast = m_VerticalLinecastEndPoint;
            if (m_UpdateCastByVerticalSpeed)
            {
                // Adjust linecast size according velocity of jump
                endCast = m_VerticalLinecastEndPoint + m_Engine.m_Rigidbody.velocity.y / 10f;
                endCast = Mathf.Clamp(endCast, 0, m_VerticalLinecastEndPoint);
            }

            Vector3 castDirection = transform.forward;

            // Overlap ledges around
            // It checks if character overlapped a ledge on his side or on his back, to allow him to climb
            // Useful in situations that character jump to back from a climb and has a ledge on side.
            if (!m_IsAlreadyClimbing)
            {
                Vector3 overlapPoint1 = transform.position + Vector3.up * m_VerticalLinecastStartPoint;
                Vector3 overlapPoint2 = transform.position + Vector3.up * endCast;

                Collider[] overlappedLedges = Physics.OverlapCapsule(overlapPoint1, overlapPoint2, m_Engine.m_Capsule.radius * 2, m_ClimbableMask, QueryTriggerInteraction.Collide);

                if(overlappedLedges.Length > 0)
                {
                    Vector3 playerClimbReference = transform.position + m_VerticalLinecastStartPoint * Vector3.up;
                    Vector3 closestPoint = overlappedLedges[0].ClosestPoint(playerClimbReference);

                    // Chose the closest ledge to player
                    foreach(Collider coll in overlappedLedges)
                    {
                        if (coll.transform.position.y > playerClimbReference.y)
                            continue;

                        Vector3 point = coll.ClosestPoint(playerClimbReference);
                        if (Vector3.Distance(playerClimbReference, point) < Vector3.Distance(playerClimbReference, closestPoint))
                            closestPoint = point;
                    }

                    closestPoint.y = playerClimbReference.y;
                    castDirection = closestPoint - playerClimbReference;
                }
            }
            
            //Set capsule points
            Vector3 capsulePoint1 = transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius) - castDirection * m_CastCapsuleRadius * 2;
            Vector3 capsulePoint2 = transform.position + Vector3.up * (endCast + m_CastCapsuleRadius) - castDirection * m_CastCapsuleRadius * 2;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///// Debug cast on Editor
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Debug.DrawLine(capsulePoint1 + Vector3.up * m_CastCapsuleRadius, capsulePoint1 + castDirection * (m_MaxDistanceToFindLedge + m_CastCapsuleRadius) + Vector3.up * m_CastCapsuleRadius);
            Debug.DrawLine(capsulePoint2 - Vector3.up * m_CastCapsuleRadius, capsulePoint2 + castDirection * (m_MaxDistanceToFindLedge + m_CastCapsuleRadius) - Vector3.up * m_CastCapsuleRadius);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///// List of edges found during Capsule Cast. 
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            List<RaycastHit> top = new List<RaycastHit>();
            List<RaycastHit> front = new List<RaycastHit>();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////
            // Cast all existing ledges in player movement and add to the list
            ////////////////////////////////////////////////
            
            RaycastHit[] hits = Physics.CapsuleCastAll(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, castDirection, m_MaxDistanceToFindLedge + m_CastCapsuleRadius, m_ClimbableMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (m_IsAlreadyClimbing)
                    AddHitsToList(hits[i], ref front, ref top);
                else
                {
                    if (hitReference == null)
                    {
                        GameObject reference = new GameObject("Climbing Ledge Reference");
                        hitReference = reference.transform;
                    }

                    hitReference.transform.forward = -hits[i].normal;

                    float delta = (m_CastCapsuleRadius * 3) / (m_Iterations*2);
                    Vector3 left = Vector3.zero;
                    Vector3 right = Vector3.zero;
                    for(int j = 0; j < m_Iterations*2; j++)
                    {
                        Vector3 checkpoint = hits[i].point + hitReference.transform.right * (-m_CastCapsuleRadius * 1.5f + delta * j);
                        
                        RaycastHit sideCheckHit;
                        if(Physics.Raycast(checkpoint + hits[i].normal * HorizontalDeltaFromLedge, -hits[i].normal, out sideCheckHit, HorizontalDeltaFromLedge * 1.5f, m_ClimbableMask))
                        {
                            if (Vector3.Angle(hits[i].normal, sideCheckHit.normal) < 45f)
                            {
                                if (hits[i].collider == sideCheckHit.collider)
                                {
                                    left = (left == Vector3.zero) ? checkpoint : left;
                                    right = checkpoint;
                                }
                            }
                        }
                    }

                    if (Vector3.Distance(left, right) >= m_CastCapsuleRadius*2)
                    {
                        hits[i].point = (left + right) / 2;
                        AddHitsToList(hits[i], ref front, ref top);
                    }
                }

            }


            ////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////
            // Choose the ledge with the lowest y position
            ////////////////////////////////////////////////

            for (int i = 0; i < top.Count; i++)
            {
                if (i == 0 || top[i].point.y < topHit.point.y)
                {
                    capsuleHit = front[i];
                    topHit = top[i];
                    if(hitReference != null)
                        hitReference.transform.forward = -capsuleHit.normal;
                }
            }

            if (top.Count != 0) { return true; }

            ///////////////////////////////////////////////

            return false; // Return false if find nothing
        }

        public void AddHitsToList(RaycastHit hit, ref List<RaycastHit> front, ref List<RaycastHit> top)
        {
            if (Physics.CheckSphere(hit.point, 0.05f, m_LadderMask))
                return;

            if (Mathf.Abs(hit.normal.y) < 0.3f && hit.point.y > transform.position.y)
            {
                if (CastLedgeFromTop(hit, out topHit)) // Ledge found
                {
                    // Add current ledge to the list of ledges
                    top.Add(topHit);
                    front.Add(hit);
                }
            }
        }


        /// <summary>
        /// After find a ledge, cast from top to check height of ledge
        /// </summary>
        /// <param name="frontHit"> Hit from capsule cast</param>
        /// <param name="topHit"> Hit to get information from linecast</param>
        /// <returns></returns>
        protected bool CastLedgeFromTop(RaycastHit frontHit, out RaycastHit topHit)
        {
            Vector3 directionToCheckLedge = (frontHit.point - transform.position); // Get direction from player to point that found ledge
            directionToCheckLedge.y = 0;

            topHit = new RaycastHit(); // Initialize topHit
            float step = m_MaxDistanceToFindLedge / m_Iterations;

            List<RaycastHit> hits = new List<RaycastHit>();

            for (int i = 0; i < m_Iterations; i++)
            {
                float endCast = m_VerticalLinecastEndPoint;

                if (m_UpdateCastByVerticalSpeed)
                {
                    // Adjust linecast size according velocity of jump
                    endCast = m_VerticalLinecastEndPoint + m_Engine.m_Rigidbody.velocity.y / 10f;
                    endCast = Mathf.Clamp(endCast, 0, m_VerticalLinecastEndPoint);
                }

                //Sets start point and endpoint of linecast
                Vector3 Start = transform.position + Vector3.up * m_VerticalLinecastStartPoint + directionToCheckLedge.normalized * step * i;
                Vector3 End = transform.position + Vector3.up * endCast + directionToCheckLedge.normalized * step * i;

                Debug.DrawLine(Start, End, Color.red);

                hits.AddRange(Physics.RaycastAll(Start, Vector3.down, Start.y - End.y, m_ClimbableMask));
            }

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == frontHit.transform && hit.normal.y > 0.8f) // Cast ledge
                {
                    topHit = hit;
                    return true;
                }
            }

            return false;
        }









        /// <summary>
        /// Set character position after find ledge
        /// </summary>
        /// <param name="frontHit">Hit from capsule cast</param>
        /// <param name="topHit">Hit from line cast</param>
        protected void SetCharacterPositionOnLedge()
        {
            transform.position = CalculatePositionOnLedge();
            transform.rotation = CalculateRotation();
        }




        private Vector3 CalculatePositionOnLedge()
        {
            Vector3 horizontalVelocity = m_Engine.m_Rigidbody.velocity;
            horizontalVelocity.y = 0;

            Vector3 newPos = frontHit.point + frontHit.normal * (m_ForwardDeltaFromLedge + horizontalVelocity.magnitude * Time.fixedDeltaTime); // Set horizontal position on ledge
            newPos.y = topHit.point.y - m_VerticalDeltaFromLedge; // Set vertical position on ledge
            
            return newPos;
        }

        private Quaternion CalculateRotation()
        {
            Vector3 direction = frontHit.normal; // Get direction of ledge
            return GetRotationFromDirection(direction, 180f); // Rotate character to face ledge
        }

        

        private bool m_UpdatePosition = false;
        private bool m_IsPositionating = false;

        private float positionStep = 0;
        private float timeCounter = 0;

        Vector3 desiredPosition = Vector3.zero;
        Quaternion desiredRot = Quaternion.identity;

        private void UpdateCharPosition()
        {
            if (!m_UpdatePosition)
                return;

            if (!m_IsPositionating)
            {
                desiredPosition = CalculatePositionOnLedge();
                desiredRot = CalculateRotation();

                positionStep = Vector3.Distance(transform.position, desiredPosition) / m_PositioningSmoothnessTime;
                timeCounter = 0;
            }

            m_IsPositionating = true;
            m_Engine.IsCoroutinePlaying = true;

            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, positionStep * Time.fixedDeltaTime); // Set position
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, 0.1f);
            timeCounter += Time.fixedDeltaTime;

            if (Mathf.Approximately(Vector3.Distance(transform.position, desiredPosition), 0) || timeCounter >= m_PositioningSmoothnessTime)
            {
                m_IsPositionating = false;
                m_UpdatePosition = false;
                m_Engine.m_Rigidbody.isKinematic = false;
                m_Engine.IsCoroutinePlaying = false;
            }
        }


        /// <summary>
        /// Check obstacles above the ledge to allow or not climb up
        /// </summary>
        /// <returns></returns>
        protected bool FreeAboveLedge()
        {
            //Set capsule points
            Vector3 capsulePoint1 = topHit.point + Vector3.up * (m_CastCapsuleRadius + 0.1f) - transform.forward * m_CastCapsuleRadius * 2;
            Vector3 capsulePoint2 = capsulePoint1 + Vector3.up * (m_Engine.m_Capsule.height * 0.5f - m_CastCapsuleRadius * 2);

            Vector3 groundCheckStart = topHit.point + Vector3.up + transform.forward * m_Engine.m_Capsule.radius * 2f;
            bool hasGround = Physics.Raycast(groundCheckStart, Vector3.down, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            return !Physics.CapsuleCast(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, -frontHit.normal, m_CastCapsuleRadius * 4, Physics.AllLayers) && hasGround;
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * (m_VerticalLinecastEndPoint + m_CastCapsuleRadius) + transform.forward * m_MaxDistanceToFindLedge, m_CastCapsuleRadius);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius) + transform.forward * m_MaxDistanceToFindLedge, m_CastCapsuleRadius);
        }
    }


}
