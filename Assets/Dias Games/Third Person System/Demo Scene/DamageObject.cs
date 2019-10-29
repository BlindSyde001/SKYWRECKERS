using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class DamageObject : MonoBehaviour
    {
        [SerializeField] private float m_DamageAmount = 10f;

        private void OnTriggerEnter(Collider other)
        {
            GlobalEvents.ExecuteEvent("Damage", other.gameObject, m_DamageAmount);
        }
    }
}