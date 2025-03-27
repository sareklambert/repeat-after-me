using System.Collections;
using Cinemachine;
using UnityEngine;
using Plamb.Events;
using Plamb.Character;

namespace Plamb.System
{
	/// <summary>
	/// This class defines the main game manager. It holds global references and handles recording of characters.
	/// </summary>
	public class PlambGameManager : Singleton<PlambGameManager>
	{
		// Define public accessors
		public bool IsRecording { get; private set; }
		public PlambComponentInput PlambInput { get; private set; }
		public PlambComponentController CurrentCharacter { get; private set; }
		public PlambComponentController OriginalCharacter { get; private set; }
		
		// Define private internal members
		[SerializeField] private PlambGameManagerSettings gameManagerSettings;
		private CinemachineVirtualCamera m_cameraController;
		private PlambCharacterPool m_characterPool;
		private Coroutine m_recordingTimerCoroutine;
		
		private void OnEnable()
		{
			// Subscribe to events
			EventBus.Subscribe<EventInputRecord>(OnRecordInput);
			EventBus.Subscribe<EventInputDeleteLastRecord>(OnRecordDeleteInput);
			EventBus.Subscribe<EventDeleteCharacterByID>(OnDeleteCharacterByID);
			
			// Get references
			PlambInput = GetComponentInChildren<PlambComponentInput>();
			m_cameraController = FindObjectOfType<CinemachineVirtualCamera>();
			
			CurrentCharacter = FindObjectOfType<PlambComponentController>();
			OriginalCharacter = CurrentCharacter;
			OriginalCharacter.Initialize();
			
			// Initialize character object pool
			m_characterPool = GetComponent<PlambCharacterPool>();
			m_characterPool.Initialize(gameManagerSettings);
		}
		private void OnDisable()
		{
			// Unsubscribe from events
			EventBus.Unsubscribe<EventInputRecord>(OnRecordInput);
			EventBus.Unsubscribe<EventInputDeleteLastRecord>(OnRecordDeleteInput);
			EventBus.Unsubscribe<EventDeleteCharacterByID>(OnDeleteCharacterByID);
		}

		// Received the record input event
		private void OnRecordInput(EventInputRecord e)
		{
			// Toggle character recording
			IsRecording = !IsRecording;
			
			if (IsRecording)
			{
				// Make sure we're not spawning more clones than allowed
				if (m_characterPool.ActiveCharacters.Count + 1 < gameManagerSettings.maxClones)
				{
					// Start recording
					RecordStart();
				}
				else
				{
					IsRecording = false;
				}
			}
			else
			{
				// Stop recording
				RecordStop();
			}
		}

		// Received the record delete input event
		private void OnRecordDeleteInput(EventInputDeleteLastRecord e)
		{
			// Make sure there is at least one clone to delete
			if (m_characterPool.ActiveCharacters.Count < 1) return;

			// Delete the newest clone in the list
			PlambComponentController clone = m_characterPool.ActiveCharacters[^1];
			DeleteCharacter(clone);
		}

		// Received the delete character by id event
		private void OnDeleteCharacterByID(EventDeleteCharacterByID e)
		{
			// Search character list for character id
			foreach (PlambComponentController c in m_characterPool.ActiveCharacters)
			{
				// Get the clone with the event's character ID
				if (c.CharacterID != e.CharacterID) continue;
				PlambComponentController clone = c;
				
				// Delete that clone
				DeleteCharacter(clone);
				break;
			}
		}

		// Deletes a clone
		private void DeleteCharacter(PlambComponentController clone)
		{
			// Stop recording if the deleted character was the currently recording one
			if (IsRecording && CurrentCharacter.CharacterID == clone.CharacterID) {
				RecordStop();
			}
				
			// Release clone to pool
			m_characterPool.Pool.Release(clone);
		
			// Raise character died event
			EventBus.Publish(new EventCharacterDied(clone.CharacterID));
		}
		
		// Spawns a new clone and starts recording
		private void RecordStart()
		{
			// Start the record timer coroutine
			m_recordingTimerCoroutine = StartCoroutine(RecordingTimer());
			
			// Get a clone from the character pool and initialize it
			PlambComponentController clone = m_characterPool.Pool.Get();
			clone.Initialize();
			CurrentCharacter = clone;
			
			// Give control to the clone and prevent the original character from moving
			CurrentCharacter.isClone = false;
			OriginalCharacter.isClone = true;
			
			// Record start event
			EventBus.Publish(new EventRecordStart(CurrentCharacter.CharacterID));
			
			// Set camera target to new clone
			m_cameraController.Follow = CurrentCharacter.CameraTarget;
		}

		// Stops recording and gives control back to the original character
		private void RecordStop()
		{
			// Stop record timer coroutine
			if (m_recordingTimerCoroutine != null)
			{
				StopCoroutine(m_recordingTimerCoroutine);
				m_recordingTimerCoroutine = null;
			}
			
			// Set recording flag
			IsRecording = false;
			
			// Record the current inputs as end point
			CurrentCharacter.PlambRecorder.ForceRecordInput(new PlambInputSnapshot());
		
			// Drop item if we have one
			if (CurrentCharacter.item) CurrentCharacter.item.Drop(CurrentCharacter);
			
			// Record stop event
			EventBus.Publish(new EventRecordStop(CurrentCharacter.CharacterID));
			
			// Give control back to the original character and let the new clone begin its loop
			CurrentCharacter.isClone = true;
			OriginalCharacter.isClone = false;
			CurrentCharacter = OriginalCharacter;
			
			// Set camera target back to original character
			m_cameraController.Follow = CurrentCharacter.CameraTarget;
		}
		
		// Keeps track of the current recording time and terminates recording if we reach the time limit
		private IEnumerator RecordingTimer()
		{
			float timer = 0f;
			while (IsRecording && timer < gameManagerSettings.maxLoopDuration)
			{
				timer += Time.deltaTime;
				yield return null;
			}
            
			if (IsRecording)
			{
				RecordStop();
			}
		}
	}
}
