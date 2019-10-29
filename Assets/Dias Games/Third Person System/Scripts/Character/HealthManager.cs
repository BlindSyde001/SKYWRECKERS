using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    [DisallowMultipleComponent]
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private float m_MaxHealth = 100f;
        [SerializeField] private float m_WaitToRestart = 1f;

        private float m_CurrentHealth;
        private Rigidbody[] ragdollRigidbodies;

        public float HealthValue { get { return m_CurrentHealth; } }
        public float MaximumHealth { get { return m_MaxHealth; } }

        private ThirdPersonSystem m_Engine;

        private Vector3 spawnPosition;
        private Quaternion spawnRotation;

        string initialState;

        private void Awake()
        {            
            m_Engine = GetComponent<ThirdPersonSystem>();

            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            m_CurrentHealth = m_MaxHealth;

            GlobalEvents.AddEvent("Damage", Damage);
            GlobalEvents.AddEvent("Restart", Initialize);
        }

        public void Die()
        {
            m_Engine.enabled = false;
            m_Engine.m_Animator.enabled = false;
            m_Engine.m_Capsule.sharedMaterial = null;

            Vector3 vel = ragdollRigidbodies[0].velocity;
            ragdollRigidbodies[0].isKinematic = true;
            
            for (int i = 1; i < ragdollRigidbodies.Length; i++)
            {
                ragdollRigidbodies[i].useGravity = true;
                ragdollRigidbodies[i].velocity = vel;
            }

            StartCoroutine(RestartCharacter());
        }

        private IEnumerator RestartCharacter()
        {
            yield return new WaitForSeconds(m_WaitToRestart);

            GlobalEvents.ExecuteEvent("Restart", null, null);
            
        }

        private void Initialize(GameObject obj, object value)
        {
            m_Engine.enabled = true;
            m_Engine.m_Animator.enabled = true;
                        
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            m_CurrentHealth = m_MaxHealth;

            ragdollRigidbodies[0].isKinematic = false;

            GlobalEvents.ExecuteEvent("OnHealthChanged", gameObject, m_CurrentHealth);
        }

        private void Damage(GameObject obj, object amount)
        {
            if (obj != gameObject)
                return;

            m_CurrentHealth -= (float)amount;
            if(m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                Die();
            }

            GlobalEvents.ExecuteEvent("OnHealthChanged", obj, m_CurrentHealth);
        }
    }
}
