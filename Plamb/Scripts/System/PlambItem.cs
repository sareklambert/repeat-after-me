using System.Collections.Generic;
using UnityEngine;
using Plamb.Events;
using Plamb.Character;

namespace Plamb.System
{
	/// <summary>
	/// This class defines a generic item for character interaction.
	/// </summary>
	public class PlambItem : MonoBehaviour
    {
	    // Keep track of colliders which entered
    	private List<Collider> m_colliderList = new List<Collider>();
    	private bool m_isGrabbed;
	    
	    // Component references
	    private Rigidbody m_rigidBody;
	    private BoxCollider[] m_colliders;
	    
    	private void OnEnable()
    	{
    		// Get references
		    m_rigidBody = GetComponent<Rigidbody>();
		    m_colliders = GetComponentsInChildren<BoxCollider>();
		    
		    // Subscribe to events
		    EventBus.Subscribe<EventPlayerInteract>(OnPlayerInteract);
	    }
    	private void OnDisable()
    	{
		    // Unsubscribe from events
    		EventBus.Unsubscribe<EventPlayerInteract>(OnPlayerInteract);
    	}
    	
    	// Add colliders to our list
    	private void OnTriggerEnter(Collider other)
    	{
    		if (m_isGrabbed) return;
    		
    		if (!m_colliderList.Contains(other) && other.gameObject.CompareTag("Character")) {
    			m_colliderList.Add(other);
    		}
    	}
    
    	// Remove colliders from our list
    	private void OnTriggerExit(Collider other)
    	{
    		if (m_isGrabbed) return;
    		
    		if (m_colliderList.Contains(other)) {
    			m_colliderList.Remove(other);
    		}
    	}
    
	    // Received the interact input event
    	private void OnPlayerInteract(EventPlayerInteract e)
    	{
		    // Check if our colliders list contains the character with the ID which triggered the event
    		foreach (Collider c in m_colliderList)
    		{
    			PlambComponentController currentCharacter = c.GetComponent<PlambComponentController>();
    			if (currentCharacter.CharacterID != e.CharacterID) continue;
    			
			    // Grab the item
    			Grab(currentCharacter);
    			break;
    		}
    	}
    
	    // A character drops the item and detaches it from its hand
    	public void Drop(PlambComponentController character)
    	{
		    // Set the grabbed flag and reset transforms
    		m_isGrabbed = false;
		    transform.parent = null;
		    transform.position = character.ItemDropPos.position;
    		
		    // Enable rigidbody and colliders
		    m_rigidBody.isKinematic = false;
		    foreach (BoxCollider c in m_colliders) c.enabled = true;
		    
		    // Reset the character's item reference
    		character.item = null;
    	}
    
	    // A character picks up the item and attaches it to its hand
    	private void Grab(PlambComponentController character)
    	{
		    // Set the grabbed flag and transforms
    		m_isGrabbed = true;
		    transform.parent = character.ItemInHandPosition;
		    transform.position = transform.parent.position;
		    transform.rotation = transform.parent.rotation;
		    
		    // Disable rigidbody and colliders
		    m_rigidBody.isKinematic = true;
		    foreach (BoxCollider c in m_colliders) c.enabled = false;
		    
		    // Set the character's item reference
		    character.item = this;
    	}
    }
}
