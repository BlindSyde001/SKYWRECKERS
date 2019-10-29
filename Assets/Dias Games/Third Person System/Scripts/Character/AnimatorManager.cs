﻿/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class AnimatorManager : MonoBehaviour
    {
        // ----- Constant variables ------ //

        public static int BaseLayerIndex { get { return 0; } } // Index of base layer
        public static int UpperLayerIndex { get { return 1; } } // Index of upper body layer
        public static int RightArm { get { return 2; } } // Index of right arm layer
        public static int LeftArm { get { return 3; } } // Index of left arm layer

        // ------------------------------- //


        // Private exposed parameters ----------------------------------------------------------------------------------------- //

        [Tooltip("Default transition duration between animations")] [SerializeField] private float m_TransitionDuration = 0.1f;
        [Space()]
        [Tooltip("Name of the forward parameter of the animator")] [SerializeField] private string m_ForwardAnimatorParameter = "Forward";
        [Tooltip("Name of the turn parameter of the animator")] [SerializeField] private string m_TurnAnimatorParameter = "Turn";
        [Tooltip("Name of the horizontal parameter of the animator")] [SerializeField] private string m_VerticalAnimatorParameter = "Vertical";
        [Tooltip("Name of the horizontal parameter of the animator")] [SerializeField] private string m_HorizontalAnimatorParameter = "Horizontal";

        // -------------------------------------------------------------------------------------------------------------------- //


        // Internal parameters and components ------------------------- //

        private Animator m_Animator; // Component for Animator attached to character

        // ------------------------------------------------------------ //

        public const int LayersCount = 4;

        private float[] layerWeight = new float[LayersCount];
        private float[] layerVelocity = new float[LayersCount];

        // Use this for initialization
        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }


        private void FixedUpdate()
        {
            for (int i = 0; i < m_Animator.layerCount; i++)
            {
                layerWeight[i] += layerVelocity[i] * Time.deltaTime;
                layerWeight[i] = Mathf.Clamp(layerWeight[i], 0, 1);
                m_Animator.SetLayerWeight(i, layerWeight[i]);
            }
        }



        /// <summary>
        /// Set a new state in animator with default parameters
        /// </summary>
        /// <param name="newState">Name of the new state</param>
        public void SetAnimatorState(string newState)
        {
            SetAnimatorState(newState, m_TransitionDuration, BaseLayerIndex);
        }




        /// <summary>
        /// Set a new state in animator with custom parameters
        /// </summary>
        /// <param name="newState">Name of the new state</param>
        /// <param name="transitionDuration">Duration of the transition</param>
        /// <param name="layer">Animator layer to make transition</param>
        public void SetAnimatorState(string newState, float transitionDuration, int layer)
        {
            layerVelocity[layer] = (1 - m_Animator.GetLayerWeight(layer)) / transitionDuration;
            m_Animator.CrossFadeInFixedTime(newState, transitionDuration, layer);
        }

        public void DisableLayer(string newState, float transitionDuration, int layer)
        {
            layerVelocity[layer] = (0 - m_Animator.GetLayerWeight(layer)) / transitionDuration;
            m_Animator.CrossFadeInFixedTime(newState, transitionDuration, layer);
        }

       
        /// <summary>
        /// Set a float parameter with custom dampTime
        /// </summary>
        /// <param name="parameter">Float paramater that must be changed</param>
        /// <param name="value">New value</param>
        /// <param name="dampTime">Damp time: higher values results in smoother change</param>
        public void SetFloatParameter(string parameter, float value, float dampTime)
        {
            m_Animator.SetFloat(parameter, value, dampTime, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Set a float parameter
        /// </summary>
        /// <param name="parameter">Float paramater that must be changed</param>
        /// <param name="value">New value</param>
        public void SetFloatParameter(string parameter, float value)
        {
            m_Animator.SetFloat(parameter, value);
        }

        /// <summary>
        /// Set forward parameter with default transition duration as damp time
        /// </summary>
        /// <param name="value">New forward value</param>
        public void SetForwardParameter(float value)
        {
            m_Animator.SetFloat(m_ForwardAnimatorParameter, value, m_TransitionDuration, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Set forward parameter
        /// </summary>
        /// <param name="value">New forward value</param>
        /// <param name="dampTime">Damp time: higher values results in smoother change</param>
        public void SetForwardParameter(float value, float dampTime)
        {
            m_Animator.SetFloat(m_ForwardAnimatorParameter, value, dampTime, Time.fixedDeltaTime);
        }


        /// <summary>
        /// Set horizontal parameter with default transition duration as damp time
        /// </summary>
        /// <param name="value">New Horizontal value</param>
        public void SetHorizontalParameter(float value)
        {
            m_Animator.SetFloat(m_HorizontalAnimatorParameter, value, m_TransitionDuration, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Set Horizontal parameter
        /// </summary>
        /// <param name="value">New Horizontal value</param>
        /// <param name="dampTime">Damp time: higher values results in smoother change</param>
        public void SetHorizontalParameter(float value, float dampTime)
        {
            m_Animator.SetFloat(m_HorizontalAnimatorParameter, value, dampTime, Time.fixedDeltaTime);
        }


        /// <summary>
        /// Set vertical parameter with default transition duration as damp time
        /// </summary>
        /// <param name="value">New Vertical value</param>
        public void SetVerticallParameter(float value)
        {
            m_Animator.SetFloat(m_VerticalAnimatorParameter, value, m_TransitionDuration, Time.fixedDeltaTime);
        }


        /// <summary>
        /// Set Vertical parameter
        /// </summary>
        /// <param name="value">New Vertical value</param>
        /// <param name="dampTime">Damp time: higher values results in smoother change</param>
        public void SetVerticallParameter(float value, float dampTime)
        {
            m_Animator.SetFloat(m_VerticalAnimatorParameter, value, dampTime, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Sets turn value
        /// </summary>
        /// <param name="vSpeed">Turn amount</param>
        /// <param name="dampTime">Damp time: higher values results in smoother change</param>
        public void SetTurnParameter(float turn, float dampTime)
        {
            m_Animator.SetFloat(m_TurnAnimatorParameter, turn, dampTime, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Sets turn value
        /// </summary>
        /// <param name="vSpeed">Turn amount</param>
        public void SetTurnParameter(float turn)
        {
            SetTurnParameter(turn, m_TransitionDuration);
        }


        public bool HasFinishedAnimation(string state)
        {
            return HasFinishedAnimation(state, BaseLayerIndex);
        }

        public bool HasFinishedAnimation(string state, int layer, bool includeLoop=false)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(layer).IsName(state))
            {
                // All looped animation should be threat with no end
                if (m_Animator.GetCurrentAnimatorStateInfo(layer).loop && !includeLoop)
                    return false;

                if (m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1 >= 0.9f)
                    return true;
            }

            return false;
        }


        public float GetNormalizedTime(int layer = 0)
        {
            return m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }
    }
}
