using UnityEngine;
using Plamb.System;

namespace Plamb.Character
{
	/// <summary>
	/// This class defines the main player controller component.
	/// It contains and handles all subcomponents.
	/// </summary>
	public class PlambComponentController : MonoBehaviour
	{
		// Private members
		private PlambInputSnapshot m_currentInput;
		private PlambInputSnapshot m_blankInput;
		
		// Component and transform references
		public PlambComponentInput PlambInput { get; private set; }
		public PlambComponentRecording PlambRecorder { get; private set; }
		public PlambComponentMovement PlambMovement { get; private set; }
		[SerializeField] private Transform itemDropPos;
		[SerializeField] private Transform cameraTarget;
		[SerializeField] private Transform itemInHandPosition;
		
		// Public members and accessors
		[HideInInspector] public bool isClone;
		[HideInInspector] public PlambItem item;
		public int CharacterID { get; private set; }
		public Transform ItemDropPos => itemDropPos;
		public Transform CameraTarget => cameraTarget;
		public Transform ItemInHandPosition => itemInHandPosition;

		// Core character loop; Handles input, physics and record / replay functionality
		private void FixedUpdate()
		{
			// Get current input
			if (!isClone)
			{
				// Get snapshot from input component
				PlambInput.PopulateInputSnapshot(ref m_currentInput);

				// Record inputs
				if (PlambGameManager.Instance.IsRecording && CharacterID != 0)
				{
					PlambRecorder.RecordInput(m_currentInput);
				}
			}
			else
			{
				// Play recorded inputs
				if (PlambRecorder.RecordListCount > 0 && CharacterID != 0)
				{
					PlambRecorder.GetInputFromRecord(ref m_currentInput);
				}
				else
				{
					// Prevent the original character from moving while recording a new clone
					m_currentInput = m_blankInput;
				}
			}
			
			// Handle physics based on input
			PlambMovement.HandlePhysics(m_currentInput);
		}
		
		// Sets the character ID, used when instantiating to the pool
		public void SetID (int id) => CharacterID = id;
		
		// Clears the current inputs
		public void ClearInputs() => m_currentInput = m_blankInput;

		// Initializes references and populate spawn settings
		public void Initialize()
		{
			// Get references
			PlambInput = PlambGameManager.Instance.PlambInput;
			PlambRecorder = GetComponent<PlambComponentRecording>();
			PlambMovement = GetComponent<PlambComponentMovement>();
			
			// Reset recorder
			PlambRecorder.Reset();
		}
	}
}
