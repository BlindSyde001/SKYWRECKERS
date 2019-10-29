using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiasGames.ThirdPersonSystem.UI
{
    public class HealthBarUIManager : MonoBehaviour
    {
        [SerializeField] private HealthManager m_Character;
        [SerializeField] private Image m_HealhtBarImage;
        
        private void Awake()
        {
            if (m_Character == null)
                m_Character = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>();
            
            GlobalEvents.AddEvent("OnHealthChanged", UpdateHealthBar);
        }

        private void UpdateHealthBar(GameObject obj, object currentHealth)
        {
            if (obj != m_Character.gameObject)
                return;

            m_HealhtBarImage.fillAmount = ((float)currentHealth) / m_Character.MaximumHealth;
        }
    }
}