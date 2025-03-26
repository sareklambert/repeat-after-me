using System.Collections.Generic;
using UnityEngine;
using Plamb.Events;
using Plamb.System;

namespace Plamb.Character
{
	/// <summary>
	/// This struct holds the position, rotation and movement component's physics values for resetting them on respawn.
	/// </summary>
	public struct SpawnSettings
	{
		// Spawn position
		public Vector3 position;
		public Quaternion rotation;

		// Physics
		public bool isGrounded;
		public Vector3 velocity;
		public float currentDirection;
		public float rotationVelocity;
		public float jumpLedgeToleranceTimer;
	}
	
	/// <summary>
	/// This component handles the recording and replaying functionality.
	/// </summary>
	public class PlambComponentRecording : MonoBehaviour
	{
		private const int PREALLOCATE_RECORD_SIZE = 256;
		
		// Private members
		private float m_recordStartTime;
		private float m_replayStartTime;
		private int m_replayIndex = 0;
		private List<PlambRecord> m_recordList = new List<PlambRecord>(PREALLOCATE_RECORD_SIZE);
		private SpawnSettings m_spawnSettings;
		
		// References
		private PlambComponentController m_plambComponentController;
		private PlambComponentMovement m_plambComponentMovement;
		
		// Public accessors
		public int RecordListCount => m_recordList.Count;

		private void OnEnable()
		{
			m_plambComponentController = GetComponent<PlambComponentController>();
			m_plambComponentMovement = GetComponent<PlambComponentMovement>();
		}
		
		// Retrieves input snapshots from the record list once their timestamps are reached; loops infinitely
		public void GetInputFromRecord(ref PlambInputSnapshot snapshot)
		{
			// Get elapsed time
			float elapsedTime = Time.time - m_replayStartTime;

			// Get current input snapshot from record list the timestamp is reached
			if (m_recordList[m_replayIndex].Timestamp <= elapsedTime)
			{
				snapshot = m_recordList[m_replayIndex].Input;
				m_replayIndex++;
			}

			// Respawn when we reach the last index
			if (m_replayIndex < m_recordList.Count) return;
			Respawn();
		}

		// Saves the input snapshots to the record list for later replaying
		public void RecordInput(PlambInputSnapshot snapshot)
		{
			float currentTime = Time.time;

			// If no records exist, initialize recording
			if (m_recordList.Count == 0)
			{
				m_recordStartTime = currentTime;
				AddRecord(0f, snapshot);
				return;
			}

			// Only record if input has changed
			if (m_recordList[^1].Input == snapshot) return;

			float timestamp = currentTime - m_recordStartTime;
			AddRecord(timestamp, snapshot);
		}

		// Public function to manually force an input record
		public void ForceRecordInput(PlambInputSnapshot snapshot)
		{
			float timestamp = Time.time - m_recordStartTime;
			AddRecord(timestamp, snapshot);
		}
 
		// Handles adding a new record to the list
		private void AddRecord(float timestamp, PlambInputSnapshot input)
		{
			m_recordList.Add(new PlambRecord(timestamp, input));
		}

		// Resets the recording component's values and populates the spawn settings for later respawning
		public void Reset()
		{
			// Reset recording variables
			m_recordList.Clear();
			m_replayIndex = 0;
			m_replayStartTime = Time.time;
			
			// Set spawn settings
			PlambGameManager.Instance.OriginalCharacter.PlambMovement.PopulateSpawnSettings(ref m_spawnSettings);
			
			// Respawn
			Respawn();
		}
		
		// Resets current transform and physics data to stored spawn settings
		private void Respawn()
		{
			// Note: Since we use the CharacterController component, the ProjectSettings > Physics > AutoSyncTransforms
			// option must be enabled to manually reset the transform like this!
			transform.SetPositionAndRotation(m_spawnSettings.position, m_spawnSettings.rotation);

			// Reset movement component physics values
			m_plambComponentMovement.OverrideWithSpawnSettings(m_spawnSettings);
			
			// Reset replay loop
			if (m_recordList.Count <= 0) return;
			m_replayIndex = 0;
			m_replayStartTime = Time.time;
			m_plambComponentController.ClearInputs();

			// Drop item if we have one
			if (m_plambComponentController.item) m_plambComponentController.item.Drop(m_plambComponentController);

			// Loop reset event
			EventBus.Publish(new EventPlayerLoopReset(m_plambComponentController.CharacterID));
		}
	}
}
