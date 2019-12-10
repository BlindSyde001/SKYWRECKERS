/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiasGames.ThirdPersonSystem
{
    public class InputButton
    {
        public bool WasPressed { get; private set; }
        public bool WasReleased { get; private set; }
        public bool IsPressed { get; private set; }

        public string InputName;

        private bool hasPressed = false;
        private float lastTime = 0;

        public InputButton(string input)
        {
            WasPressed = false;
            WasReleased = false;
            IsPressed = false;

            InputName = input;
        }

        public void Update(bool weponButton = false)
        {
            if (weponButton)
            {
                IsPressed = Input.GetButton(InputName);
                WasPressed = Input.GetButtonDown(InputName);
                WasReleased = Input.GetButtonUp(InputName);
                return;
            }

            IsPressed = Input.GetButton(InputName);

            if (lastTime < Time.fixedTime)
            {
                lastTime = Time.fixedTime + Time.maximumDeltaTime;
                WasPressed = false;
                WasReleased = false;
            }

            if (IsPressed)
            {
                //still holding
                if (!hasPressed)
                {
                    WasPressed = true;
                    WasReleased = false;
                    hasPressed = true;
                }
            }
            else
            {
                //has let button go
                if (hasPressed)
                {
                    WasPressed = false;
                    WasReleased = true;
                    hasPressed = false;
                }
            }
        }


        public void SetButtonState(bool wasPressed, bool wasReleased, bool pressing)
        {
            WasPressed = wasPressed;
            WasReleased = wasReleased;
            IsPressed = pressing;
        }
    }

    public class UnityInputManager : MonoBehaviour
    {        
        // Camera reference
        [Tooltip("Camera used in the scene")][SerializeField] private Transform m_Camera;
        
        // --------------------- INPUT BUTTONS --------------------- //

        public InputButton jumpButton { get; private set; }
        public InputButton walkButton { get; private set; }
        public InputButton rollButton { get; private set; }
        public InputButton crouchButton { get; private set; }
        public InputButton dropButton { get; private set; }

        public InputButton toggleWeaponButton { get; private set; }
        public InputButton rightWeaponButton { get; private set; }
        public InputButton leftWeaponButton { get; private set; }
        public InputButton zoomButton { get; private set; }
        public InputButton fireButton { get; private set; }
        public InputButton reloadButton { get; private set; }

        public InputButton interactButton { get; private set; }

        // String names
        [Space()]
        [SerializeField] private string m_JumpInputName = "Jump";
        [Space()]
        [SerializeField] private string m_WalkInputName = "Walk";
        [Space()]
        [SerializeField] private string m_RollInputName = "Roll";
        [Space()]
        [SerializeField] private string m_DropInputName = "Drop";
        [Space()]
        [SerializeField] private string m_CrouchInputName = "Crouch";
        [Space()]
        [SerializeField] private string m_ZoomInputName = "Zoom";
        [Space()]

        [SerializeField] private string m_InteractInputName = "Interact";
        [Space()]
        [SerializeField] private string m_ToggleWeaponInputName = "Toggle";
        [Space()]
        [SerializeField] private string m_RightWeaponInputName = "RightWeapon";
        [Space()]
        [SerializeField] private string m_LeftWeaponInputName = "LeftWeapon";
        [Space()]
        [SerializeField] private string m_FireInputName = "Fire";
        [Space()]
        [SerializeField] private string m_ReloadInputName = "Reload";

        // --------------------------------------------------------- //

        private Vector2 m_Move;
        private Vector3 m_RelativeInput;

        public Vector3 Move { get { return m_Move; }  set { m_Move = value; } }
        public Vector3 RelativeInput { get { return m_RelativeInput; } }

        [HideInInspector] public bool manualUpdate = false;

        private void Awake()
        {
            // Find main camera if it was not attached in hierarchy
            if (m_Camera == null)
            {
                if (Camera.main == null)
                {
                    Debug.LogError("There is no Camera in the scene. Please add a camera!");
                }
                else
                    m_Camera = Camera.main.transform;
            }

            // Initialize buttons
            jumpButton = new InputButton(m_JumpInputName);
            walkButton = new InputButton(m_WalkInputName);
            rollButton = new InputButton(m_RollInputName);
            crouchButton = new InputButton(m_CrouchInputName);
            dropButton = new InputButton(m_DropInputName);

            toggleWeaponButton = new InputButton(m_ToggleWeaponInputName);
            rightWeaponButton = new InputButton(m_RightWeaponInputName);
            leftWeaponButton = new InputButton(m_LeftWeaponInputName);
            fireButton = new InputButton(m_FireInputName);
            reloadButton = new InputButton(m_ReloadInputName);
            zoomButton = new InputButton(m_ZoomInputName);

            interactButton = new InputButton(m_InteractInputName);
        }
        
        private void FixedUpdate()
        {
            if (!manualUpdate)
            {
                m_Move.x = Input.GetAxis("Horizontal");
                m_Move.y = Input.GetAxis("Vertical");
            }
            // calculate camera relative direction to move:
            Vector3 CamForward = Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            m_RelativeInput = m_Move.y * CamForward + m_Move.x * m_Camera.right;

            if(m_Move.x != 0 || m_Move.y != 0)
            {
                print("Moving");
            }

        }

        private void Update()
        {
            if (manualUpdate)
                return;

            jumpButton.Update();
            walkButton.Update();
            rollButton.Update();
            crouchButton.Update();
            dropButton.Update();

            toggleWeaponButton.Update(true);
            rightWeaponButton.Update(true);
            leftWeaponButton.Update(true);
            fireButton.Update(true);
            reloadButton.Update(true);
            zoomButton.Update();
            interactButton.Update();
        }
    }
}
