using UnityEngine;
using UnityEngine.InputSystem;
using Plamb.Events;

namespace Plamb.Character
{
    /// <summary>
    /// This component handles player inputs.
    /// It can populate an input snapshot struct with the current inputs.
    /// </summary>
    public class PlambComponentInput : MonoBehaviour
    {
        // Define private members
        private PlayerInput m_playerInput;
        private InputAction m_inputMove;
        private InputAction m_inputJump;
        private InputAction m_inputThrow;
        private InputAction m_inputInteract;
        private float m_jumpInputRememberTimer;
        
        // Define serialized fields
        [SerializeField] private PlambSettings plambSettings;
        [SerializeField] private string actionNameMove = "Move";
        [SerializeField] private string actionNameJump = "Jump";
        [SerializeField] private string actionNameThrow = "Throw";
        [SerializeField] private string actionNameInteract = "Interact";
        
        private void Start() {
            // Get references
            m_playerInput = GetComponent<PlayerInput>();
            
            // Get input actions
            m_inputMove = m_playerInput.actions[actionNameMove];
            m_inputJump = m_playerInput.actions[actionNameJump];
            m_inputThrow = m_playerInput.actions[actionNameThrow];
            m_inputInteract = m_playerInput.actions[actionNameInteract];
        }

        // Keeps track of the input remember timer
        private void FixedUpdate()
        {
            m_jumpInputRememberTimer -= Time.deltaTime;
        }
        
        // Populates an input snapshot with the current player inputs
        public void PopulateInputSnapshot(ref PlambInputSnapshot snapshot)
        {
            // Movement vector
            snapshot.movementVector = m_inputMove.ReadValue<Vector2>();
			
            // Jump
            if (m_inputJump.IsPressed()) m_jumpInputRememberTimer = plambSettings.jumpInputRememberTime;
            snapshot.actionJump = (m_jumpInputRememberTimer > 0);
			
            // Interact
            snapshot.actionInteract = m_inputInteract.WasPressedThisFrame();
			
            // Throw
            snapshot.actionThrow = m_inputThrow.IsPressed();
        }

        // Forwards the record input to the event bus
        public void OnRecord()
        {
            EventBus.Publish(new EventInputRecord());
        }
        
        // Forwards the record delete input to the event bus
        public void OnDeleteLastRecord()
        {
            EventBus.Publish(new EventInputDeleteLastRecord());
        }
    }
}
