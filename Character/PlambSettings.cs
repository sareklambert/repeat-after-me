using UnityEngine;

namespace Plamb.Character
{
    /// <summary>
    /// This class defines the settings for the character.
    /// </summary>
    [CreateAssetMenu(fileName = "PlambSettings", menuName = "Plamb/Settings")]
    public class PlambSettings : ScriptableObject
    {
        [Header("Physics")]
        public float gravity = 15f;
        public float jumpSpeed = 36f;
        public float moveSpeed = 5.335f;
        public float accelerationSpeed = 10f;
        
        [Tooltip("The character's maximum vertical velocity.")]
        public float terminalVelocity = 53.0f;
        [Tooltip("How long the character can be mid-air and still jump.")]
        public float jumpLedgeToleranceTime = .5f;
        [Tooltip("How long a single jump input will be remembered and still trigger.")]
        public float jumpInputRememberTime = .5f;
        
        [Tooltip("How fast the character turns to face movement direction.")]
        public float rotationSmoothTime = .12f;
        
        [Tooltip("The minimum distance from the character to the ground for the grounded check to trigger.")]
        public float groundedOffset = -.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController component.")]
        public float groundedRadius = .28f;
        [Tooltip("What layers the character uses as ground.")]
        public LayerMask groundLayers;
        
        [Header("Animation")]
        public float fallAnimationVelocityThreshold = -25f;
        public float landAnimationVelocityThreshold = -5f;
    }
}
