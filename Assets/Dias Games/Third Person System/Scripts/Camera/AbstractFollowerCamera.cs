using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.Cameras
{
    public abstract class AbstractFollowerCamera : MonoBehaviour
    {

        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        [SerializeField] protected Transform m_Target;            // The target object to follow
        [SerializeField] private bool m_AutoTargetPlayer = true;  // Whether the rig should automatically target the player.
        [SerializeField] public UpdateType m_UpdateType;         // stores the selected update type

        protected Rigidbody targetRigidbody;

        protected virtual void Start()
        {
            // if auto targeting is used, find the object tagged "Player"
            // any class inheriting from this should call base.Start() to perform this action!
            if (m_AutoTargetPlayer)
            {
                FindAndTargetPlayer();
            }
            if (m_Target == null) return;
            targetRigidbody = m_Target.GetComponent<Rigidbody>();
        }


        protected virtual void FixedUpdate()
        {
            if (m_UpdateType == UpdateType.FixedUpdate && m_Target != null && m_Target.gameObject.activeSelf)
                FollowTarget(Time.deltaTime);
        }


        protected virtual void LateUpdate()
        {
            if (m_UpdateType == UpdateType.LateUpdate && m_Target != null && m_Target.gameObject.activeSelf)
                FollowTarget(Time.deltaTime);
        }


        public void ManualUpdate()
        {
            if (m_UpdateType == UpdateType.ManualUpdate && m_Target != null && m_Target.gameObject.activeSelf)
                FollowTarget(Time.deltaTime);
        }

        protected abstract void FollowTarget(float deltaTime);


        public void FindAndTargetPlayer()
        {
            // auto target an object tagged player, if no target has been assigned
            var targetObj = GameObject.FindGameObjectWithTag("Player");
            if (targetObj)
            {
                SetTarget(targetObj.transform);
            }
        }


        public virtual void SetTarget(Transform newTransform)
        {
            m_Target = newTransform;
        }


        public Transform Target
        {
            get { return m_Target; }
        }
    }
}