using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.Cameras
{
    public class CameraController : PivotRigCamera
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        #region Camera Rotation Controller

        [SerializeField] private float m_MoveSpeed = 10f;                       // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;     // How fast the rig will rotate from user input.
        [SerializeField] private float m_TurnSmoothing = 15.0f;                 // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [Range(0, 60f)][SerializeField] private float m_MaxPitch = 60f;                        // The maximum value of the x axis rotation of the pivot.
        [Range(0, 45f)] [SerializeField] private float m_MinPitch = 45f;                        // The minimum value of the x axis rotation of the pivot.
        [SerializeField] private bool m_HideCursor = false;                     // Whether the cursor should be hidden and locked.

        [Tooltip("Smoothness transition between camera positions")]
        [SerializeField] private float m_TransitionSmooth = 0.1f;               // Greater values make smoother transitions

        private float m_LookAngle;                                              // The rig's y axis rotation.
        private float m_TiltAngle;                                              // The pivot's x axis rotation.

        private Vector3 m_PivotEulers;                                          // Pivot local rotation in eulers
        private Quaternion m_PivotTargetRot;                                    // Pivot target local rotation changed by input
        private Quaternion m_TransformTargetRot;                                // Yaw target rot

        #endregion

        [HideInInspector] public Vector2 TurnAxis;
        [HideInInspector] public bool useStandardInput = true;

        [SerializeField] private string TurnXInput = "Mouse X";                 // Native input button name
        [SerializeField] private string TurnYInput = "Mouse Y";                 // Native input button name

        #region Protect from wall clip

        [SerializeField] private LayerMask m_ClipLayers = ~(1 << 15);           // Mask for clip casting. Exclude only Character layer

        [SerializeField] private float clipSpeed = 0.05f;                       // time taken to move when avoiding cliping (low value = fast, which it should be)
        [SerializeField] private float sphereCastRadius = 0.1f;                 // the radius of the sphere used to test for object between camera and target

        private Vector3 m_CameraCurrentLocalPosition;                           // Current camera position to smooth damp
        private RayHitComparer m_RayHitComparer;                                // variable to compare raycast hit distances

        #endregion

        [SerializeField] private CameraData m_DefaultCameraData;                // Default camera data
        [SerializeField] private CameraData m_DefaultZoomCameraData;             // Default aim camera data

        // Public getters
        public Camera MainCamera { get { return m_Camera; } }

        // Internal vars
        private CameraData CurrentCameraData;
        private bool IsZomming = false;
        private float Recoil = 0;
        private float recoilToUse = 0;
        private float returnRecoilVelocity;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private CameraData initialCamera;
        private Vector3 ShakeOffset = Vector3.zero;
        
        protected override void Awake()
        {
            base.Awake();

            // Lock or unlock the cursor.
            Cursor.lockState = m_HideCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_HideCursor;

            m_PivotEulers = m_Pivot.localRotation.eulerAngles;

            m_PivotTargetRot = m_Pivot.localRotation;
            m_TransformTargetRot = transform.rotation;
            m_Camera = GetComponentInChildren<Camera>();

            CurrentCameraData = m_DefaultCameraData;

            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialCamera = CurrentCameraData;

            GlobalEvents.AddEvent("Restart", Initialize);
        }

        private void Load()
        {
            m_LookAngle = transform.eulerAngles.y;
            m_TiltAngle = 0;
            SetCameraData(initialCamera);
        }

        private void Initialize(GameObject obj, object value)
        {
            if (obj != m_Target.gameObject)
                return;

            transform.position = initialPosition;
            transform.rotation = initialRotation;

            m_TransformTargetRot = initialRotation;
            m_LookAngle = initialRotation.eulerAngles.y;
            m_TiltAngle = 0;
            SetCameraData(initialCamera);
        }

        protected override void Start()
        {
            base.Start();

            m_CameraCurrentLocalPosition = Vector3.zero;
            m_CameraCurrentLocalPosition.z = CurrentCameraData.Offset.z;

            // create a new RayHitComparer
            m_RayHitComparer = new RayHitComparer();

            Initialize(m_Target.gameObject, null);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            CameraRotation(); // Rotate camera by input
            UpdateCameraData(); // Make transition between cameras data

            if (m_HideCursor && Input.GetMouseButtonUp(0)) // Lock cursor when mouse click
            {
                Cursor.lockState = m_HideCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !m_HideCursor;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            ProtectCameraFromClip();
        }

        public void AddRecoil(float recoil)
        {
            Recoil = recoil;
        }

        /// <summary>
        /// Handles all input to rotate camera around desired character
        /// </summary>
        private void CameraRotation()
        {
            if (Time.timeScale < float.Epsilon)
                return;
            if (useStandardInput)
            {
                TurnAxis.x = Input.GetAxis(TurnXInput);
                TurnAxis.y = Input.GetAxis(TurnYInput);
            }

            recoilToUse = Mathf.Lerp(recoilToUse, Recoil, 0.1f);
            float recoilMultiplier = Mathf.Clamp((1 - recoilToUse * 3f), 0.1f, 1f);
            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            m_LookAngle += TurnAxis.x * m_TurnSpeed * recoilMultiplier;

            // Rotate the rig (the root object) around Y axis only:
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= TurnAxis.y * m_TurnSpeed * recoilMultiplier + recoilToUse;
            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_MinPitch, m_MaxPitch);

            // Tilt input around X is applied to the pivot (the child of this object)
            m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);

            if (m_TurnSmoothing > 0)
            {
                m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
            }
            else
            {
                m_Pivot.localRotation = m_PivotTargetRot;
                transform.localRotation = m_TransformTargetRot;
            }

            Recoil = Mathf.SmoothDamp(Recoil, 0, ref returnRecoilVelocity, 0.1f);
        }

        /// <summary>
        /// Constantly checks which position should be the camera and automatically changes it with smothness
        /// </summary>
        private void UpdateCameraData()
        {
            // Get desired property
            Vector3 pivotTargetPosition = new Vector3(CurrentCameraData.Offset.x, CurrentCameraData.Offset.y, 0) + ShakeOffset; // Get initial target position
            Vector3 Velocity = Vector3.zero;
            float velocity = 0;

            // Smooth update pivot local position
            m_Pivot.localPosition = Vector3.SmoothDamp(m_Pivot.localPosition, pivotTargetPosition, ref Velocity, m_TransitionSmooth);

            // Smooth update camera field of view
            m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, CurrentCameraData.FieldOfView, ref velocity, m_TransitionSmooth);
        }
        
        /// <summary>
        /// Set Camera Data
        /// </summary>
        /// <param name="newCameraData"></param>
        public void SetCameraData(CameraData newCameraData = null)
        {
            if (IsZomming || stopAbilityUpdate)
                return;

            CurrentCameraData = (newCameraData == null) ? m_DefaultCameraData : newCameraData;
        }

        private bool stopAbilityUpdate = false;
        /// <summary>
        /// Set Camera Data
        /// </summary>
        /// <param name="newCameraData"></param>
        public void SetCameraArea(CameraData newCameraData = null)
        {
            if (newCameraData == null)
            {
                stopAbilityUpdate = false;
                return;
            }

            if (IsZomming)
                return;

            CurrentCameraData = (newCameraData == null) ? m_DefaultCameraData : newCameraData;
            stopAbilityUpdate = true;
        }


        public void ZoomCamera(CameraData zoomCameraData = null)
        {
            IsZomming = true;

            if (zoomCameraData != null)
                CurrentCameraData = zoomCameraData;
            else
                CurrentCameraData = m_DefaultZoomCameraData;
        }

        public void StopZoomCamera()
        {
            if (IsZomming)
                IsZomming = false;
        }

        #region Protect camera from clip

        /// <summary>
        /// Shots ray to avoid camera clip in any object.
        /// </summary>
        private void ProtectCameraFromClip()
        {
            Vector3 cameraTargetPosition = new Vector3(0, 0, CurrentCameraData.Offset.z); // Get initial target position

            Vector3 originPoint = transform.position + Vector3.up * CurrentCameraData.Offset.y; // Get pivot world position
            Vector3 targetPoint = m_Pivot.position + m_Pivot.forward * (CurrentCameraData.Offset.z + m_Camera.nearClipPlane); // Get camera world position

            // Set ray properties
            Ray m_Ray = new Ray();
            m_Ray.direction = (targetPoint - originPoint).normalized;
            m_Ray.origin = originPoint;// + m_Ray.direction * sphereCastRadius;

            // initial check to see if start of spherecast intersects anything
            var cols = Physics.OverlapSphere(m_Ray.origin, sphereCastRadius, m_ClipLayers);

            bool initialIntersect = false;
            bool hitSomething = false;

            // loop through all the collisions to check if something we care about
            for (int i = 0; i < cols.Length; i++)
            {
                if ((!cols[i].isTrigger) &&
                    cols[i].gameObject != m_Target.gameObject)
                {
                    initialIntersect = true;
                    break;
                }
            }


            RaycastHit[] m_Hits;
            // if there is a collision
            if (initialIntersect)
            {
                //m_Ray.origin += m_Ray.direction * sphereCastRadius;
                m_Ray.origin = transform.position;
                m_Ray.direction = (targetPoint - m_Ray.origin).normalized;

                // do a raycast and gather all the intersections
                m_Hits = Physics.RaycastAll(m_Ray, Mathf.Abs(CurrentCameraData.Offset.z) - sphereCastRadius, m_ClipLayers);
            }
            else
            {
                // if there was no collision do a sphere cast to see if there were any other collisions
                m_Hits = Physics.SphereCastAll(m_Ray, sphereCastRadius, Mathf.Abs(CurrentCameraData.Offset.z) + sphereCastRadius, m_ClipLayers);
            }

            // sort the collisions by distance
            Array.Sort(m_Hits, m_RayHitComparer);

            // set the variable used for storing the closest to be as far as possible
            float nearest = Mathf.Infinity;

            // loop through all the collisions
            for (int i = 0; i < m_Hits.Length; i++)
            {
                // only deal with the collision if it was closer than the previous one, not a trigger, and not attached to a rigidbody tagged with the dontClipTag
                if (m_Hits[i].distance < nearest && (!m_Hits[i].collider.isTrigger) &&
                    !(m_Hits[i].collider.attachedRigidbody != null &&
                      m_Hits[i].collider.gameObject != m_Target.gameObject))
                {
                    // change the nearest collision to latest
                    nearest = m_Hits[i].distance;

                    cameraTargetPosition = m_Ray.origin + m_Ray.direction * (nearest - sphereCastRadius); // Update camera target position
                    cameraTargetPosition = m_Pivot.InverseTransformPoint(cameraTargetPosition); // Transform desired position to local position based on Pivot

                    hitSomething = true;
                }
            }

            // visualise the cam clip effect in the editor
            if (hitSomething)
            {
                Debug.DrawRay(m_Ray.origin, m_Ray.direction * (cameraTargetPosition.magnitude + sphereCastRadius), Color.red);
            }

            // Update camera local position
            Vector3 moveVelocity = Vector3.zero;
            m_Camera.transform.localPosition = m_CameraCurrentLocalPosition = Vector3.SmoothDamp(m_CameraCurrentLocalPosition, cameraTargetPosition, ref moveVelocity,
                                                                hitSomething ? clipSpeed : m_TransitionSmooth);
        }

        // comparer for check distances in ray cast hits
        public class RayHitComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
            }
        }

        #endregion
        
        protected override void FollowTarget(float deltaTime)
        {
            if (m_Target == null) return;
            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);
        }

        public static RaycastHit GetScreenHitPoint(Camera camera, float maxDistance, LayerMask targetMask, float spread)
        {
            float screenX = Screen.width * (0.5f + UnityEngine.Random.Range(-spread, spread));
            float screenY = Screen.height * (0.5f + UnityEngine.Random.Range(-spread, spread));


            Ray aimRay = camera.ScreenPointToRay(new Vector3(screenX, screenY));

            Debug.DrawRay(aimRay.origin, aimRay.direction * 5f);

            RaycastHit hit;
            Physics.Raycast(aimRay, out hit, maxDistance, targetMask);

            return hit;
        }

        public static RaycastHit GetScreenHitPoint(Camera camera, float maxDistance, LayerMask targetMask)
        {
            return GetScreenHitPoint(camera, maxDistance, targetMask, 0);
        }

        /// <summary>
        /// Make Camera Shake
        /// </summary>
        /// <param name="duration">Duration in seconds that camera must shakes</param>
        /// <param name="intensity">The initial intensity that camera shakes.</param>
        /// <param name="OnFinishShake">Event to trigger when shake finishes</param>
        /// <param name="intensityIncrement">How much camera should increases shake during loop<</param>
        public void ShakeCamera(float duration, float intensity, Action OnFinishShake, float intensityIncrement = 0)
        {
            StartCoroutine(DoShake(duration, intensity, OnFinishShake, intensityIncrement));
        }

        public void StopShake()
        {
            StopAllCoroutines();
            ShakeOffset = Vector3.zero;
            isShaking = false;
        }


        private bool isShaking = false;
        /// <summary>
        /// Coroutine that handles all logic to shake camera during time
        /// </summary>
        /// <param name="duration">Duration in seconds that camera shakes</param>
        /// <param name="initialIntensity">The initial intensity that camera shakes.</param>
        /// <param name="OnFinishShake">Event to trigger when shake finishes</param>
        /// <param name="incrementeIntensity">How much camera should increases shake during loop</param>
        /// <returns></returns>
        private IEnumerator DoShake(float duration, float initialIntensity, Action OnFinishShake,float intensityIncrement = 0)
        {
            if (!isShaking)
            {
                isShaking = true;

                float timeToStop = Time.fixedTime + duration;
                float intensity = initialIntensity;

                while (Time.fixedTime < timeToStop)
                {
                    ShakeOffset += Vector3.right * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.left * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.up * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.down * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.left * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.right * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.down * intensity;
                    yield return new WaitForSeconds(0.01f);

                    ShakeOffset += Vector3.up * intensity;
                    yield return new WaitForSeconds(0.01f);

                    intensity += intensityIncrement;

                    yield return null;
                }

                if (OnFinishShake != null)
                    OnFinishShake();

                StopShake();

                isShaking = false;
            }
        }
    }
}
