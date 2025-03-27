using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Plamb.Character;

namespace Plamb.System
{
    /// <summary>
    /// This class defines the object pool for player characters.
    /// </summary>
    public class PlambCharacterPool : MonoBehaviour
    {
        // Define prefab and public accessors
        [SerializeField] private PlambComponentController characterPrefab;
        public ObjectPool<PlambComponentController> Pool { get; private set; }
        public List<PlambComponentController> ActiveCharacters { get; private set; } = new List<PlambComponentController>();

        // Initializes the object pool
        public void Initialize(PlambGameManagerSettings settings)
        {
            // Create the pool
            Pool = new ObjectPool<PlambComponentController>(SpawnCharacter, OnGetFromPool, OnReleaseToPool,
                OnDestroyPooledObject, true, settings.maxClones, settings.maxClones);
            
            // Pre-populate the pool
            for (int i = 0; i < settings.maxClones; i++)
            {
                // Instantiate character
                var character = SpawnCharacter();
                
                // Assign ID
                character.SetID(i + 1);
                
                // Assign material
                var materialsArray = character.GetComponentInChildren<SkinnedMeshRenderer>().materials;
                materialsArray[0] = settings.cloneMaterials[i];
                character.GetComponentInChildren<SkinnedMeshRenderer>().materials = materialsArray;
                
                // Release character to pool
                Pool.Release(character);
            }
        }
        
        // Instantiates a new object
        private PlambComponentController SpawnCharacter()
        {
            return Instantiate(characterPrefab);
        }

        // Gets an object from the pool and activates it
        private void OnGetFromPool(PlambComponentController pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
            ActiveCharacters.Add(pooledObject);
        }

        // Returns an object to the pool and deactivates it
        private void OnReleaseToPool(PlambComponentController pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            ActiveCharacters.Remove(pooledObject);
        }

        // Destroys a pooled object
        private void OnDestroyPooledObject(PlambComponentController pooledObject)
        {
            Destroy(pooledObject.gameObject);
        }
    }
}
