using UnityEngine;
using Plamb.Events;

namespace Plamb.Character
{
	/// <summary>
	/// This component handles player movement, other inputs and collisions with other characters.
	/// </summary>
	public class PlambComponentMovement : MonoBehaviour
	{
		[SerializeField] private string characterTag = "Character";
		
		// References
		private CharacterController m_characterController;
		private PlambComponentController m_plambComponentController;
		[SerializeField] private PlambSettings plambSettings;
		private Transform m_cameraTransform;

		// Physics
		private bool m_isGrounded;
		private Vector3 m_velocity = Vector3.zero;
		private float m_currentDirection;
		private float m_rotationVelocity;
		private float m_jumpLedgeToleranceTimer;
		
		private void Start()
		{
			// Get references
			m_characterController = GetComponent<CharacterController>();
			m_plambComponentController = GetComponent<PlambComponentController>();
			m_cameraTransform = FindObjectOfType<Camera>().transform;
		}

		// Populates the physics component's spawn settings
		public void PopulateSpawnSettings(ref SpawnSettings spawnSettings)
		{
			spawnSettings.position.x = transform.position.x;
			spawnSettings.position.y = transform.position.y;
			spawnSettings.position.z = transform.position.z;
			spawnSettings.rotation.eulerAngles = transform.rotation.eulerAngles;
			
			spawnSettings.isGrounded = m_isGrounded;
			spawnSettings.velocity = m_velocity;
			spawnSettings.currentDirection = m_currentDirection;
			spawnSettings.rotationVelocity = m_rotationVelocity;
			spawnSettings.jumpLedgeToleranceTimer = m_jumpLedgeToleranceTimer;
		}

		// Overrides physics values with spawn settings
		public void OverrideWithSpawnSettings(SpawnSettings spawnSettings)
		{
			m_isGrounded = spawnSettings.isGrounded;
			m_velocity = spawnSettings.velocity;
			m_currentDirection = spawnSettings.currentDirection;
			m_rotationVelocity = spawnSettings.rotationVelocity;
			m_jumpLedgeToleranceTimer = spawnSettings.jumpLedgeToleranceTimer;
		}
		
		// Handles character controller physics based on current input snapshot
		public void HandlePhysics(PlambInputSnapshot snapshot)
		{
			// Grounded check
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y -
				plambSettings.groundedOffset, transform.position.z);
			m_isGrounded = (m_velocity.y <= 0f && Physics.CheckSphere(spherePosition, plambSettings.groundedRadius,
				plambSettings.groundLayers, QueryTriggerInteraction.Ignore));

			// Handle gravity and ledge tolerance timer
			if (m_isGrounded)
			{
				// Land event
				if (m_velocity.y < 0f)
				{
					EventBus.Publish(new EventPlayerLand(m_plambComponentController.CharacterID, m_velocity.y));
				}

				m_velocity.y = 0f;
				m_jumpLedgeToleranceTimer = plambSettings.jumpLedgeToleranceTime;
			}
			else
			{
				m_velocity.y -= plambSettings.gravity * Time.deltaTime;
				m_jumpLedgeToleranceTimer -= Time.deltaTime;

				// Fall event
				EventBus.Publish(new EventPlayerFall(m_plambComponentController.CharacterID, m_velocity.y));
			}

			// Jump
			if (snapshot.actionJump && m_jumpLedgeToleranceTimer > 0)
			{
				m_velocity.y = Mathf.Sqrt(plambSettings.jumpSpeed);
				m_jumpLedgeToleranceTimer = 0;

				// Jump event
				EventBus.Publish(new EventPlayerJump(m_plambComponentController.CharacterID));
			}

			// Interact
			if (snapshot.actionInteract)
			{
				// Check if we carry an item
				if (m_plambComponentController.item)
				{
					// Drop the item
					m_plambComponentController.item.Drop(m_plambComponentController);
				}
				else
				{
					// Interact event (try to grab an item)
					EventBus.Publish(new EventPlayerInteract(m_plambComponentController.CharacterID));
				}
			}

			// Clamp vertical velocity
			m_velocity.y = Mathf.Clamp(m_velocity.y, -plambSettings.terminalVelocity,
				plambSettings.terminalVelocity);

			// Get new movement speed
			Vector3 currentHorizontalVelocity = new Vector3(m_characterController.velocity.x, 0f,
				m_characterController.velocity.z);
			float newSpeed = Mathf.Max(Mathf.Lerp(currentHorizontalVelocity.magnitude,
				snapshot.movementVector.magnitude * plambSettings.moveSpeed,
				Time.deltaTime * plambSettings.accelerationSpeed), 0);

			// Walk event
			if (m_isGrounded && newSpeed != 0f)
			{
				EventBus.Publish(new EventPlayerWalk(m_plambComponentController.CharacterID, newSpeed));
			}

			// Get new movement direction relative to camera
			if (snapshot.movementVector != Vector2.zero)
			{
				Vector3 inputDirection = new Vector3(snapshot.movementVector.x, 0.0f,
					snapshot.movementVector.y).normalized;
				m_currentDirection = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
					m_cameraTransform.eulerAngles.y;

				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_currentDirection,
					ref m_rotationVelocity, plambSettings.rotationSmoothTime);
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}

			Vector3 targetDirection = Quaternion.Euler(0.0f, m_currentDirection, 0.0f) * Vector3.forward;

			// Move the character using the character controller
			m_characterController.Move((targetDirection.normalized * newSpeed + m_velocity) * Time.deltaTime);
		}

		// Handles collision of two characters
		void OnTriggerEnter(Collider other)
		{
			// Check if we are colliding with another character
			if (!other.gameObject.CompareTag(characterTag)) return;

			// Get the other character's ID
			PlambComponentController otherPlambController = other.gameObject.GetComponent<PlambComponentController>();
			int otherID = otherPlambController.CharacterID;

			// Prevent faulty collisions
			if (m_plambComponentController.CharacterID == otherID || m_plambComponentController.CharacterID == 0) return;

			if (otherID != 0)
			{
				// 2 clones collide; Delete both
				EventBus.Publish(new EventDeleteCharacterByID(otherID));
				EventBus.Publish(new EventDeleteCharacterByID(m_plambComponentController.CharacterID));
			}
			else
			{
				// We are a clone and collide with the original character; Push the original out of the way
				Vector3 pushVector = other.gameObject.transform.position - transform.position;
				pushVector.y = 0f;
				pushVector.Normalize();

				otherPlambController.PlambMovement.m_characterController.Move(pushVector *
					plambSettings.moveSpeed * Time.deltaTime);
			}
		}
	}
}
