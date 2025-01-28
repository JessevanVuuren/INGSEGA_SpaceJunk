using UnityEngine;
using Random = UnityEngine.Random;

namespace GERV.Testing
{
    public class TestSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("The prefab to spawn.")]
        public GameObject objectToSpawn;

        [Tooltip("Spawn rate in seconds (how often to spawn objects).")]
        public float spawnRate = 1f;

        [Tooltip("The velocity of the spawned objects.")]
        public float spawnVelocity = 5f;

        [Tooltip("Offset from the spawner position.")]
        public Vector3 spawnOffset = Vector3.zero;

        private float _nextSpawnTime;
        private bool _spawnObjectIsValid = false;

        private void Start()
        {
            if (objectToSpawn == null) return;

            GameObject spawnedObject = Instantiate(
                objectToSpawn, 
                transform.position + spawnOffset, 
                transform.rotation
            );

            // Add a Rigidbody2D if it doesn't already have one
            Rigidbody2D rb = objectToSpawn.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} has no Rigidbody2D.");

                return;
            }
        
            Destroy(spawnedObject);

            this._spawnObjectIsValid = true;
        }

        void Update()
        {
            if(!this._spawnObjectIsValid) return;
        
            // Check if it's time to spawn the next object
            if (Time.time < _nextSpawnTime) return;
            {
                SpawnObject();
                _nextSpawnTime = Time.time + spawnRate; // Schedule the next spawn
            }
        }

        void SpawnObject()
        {
            // Instantiate the object at the spawner's position with the same rotation
            // GameObject spawnedObject = Instantiate(
            //     objectToSpawn, 
            //     transform.position + spawnOffset, 
            //     transform.rotation
            // );
        
            GameObject spawnedObject = Instantiate(
                objectToSpawn, 
                transform.position + spawnOffset, 
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) // Random rotation around the Z-axis
            );

            Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
        
            //mandatory check
            if (rb == null) return;

            // Set the velocity to move UP relative to the spawner's transform
            Vector2 velocityDirection = transform.up; // Local UP direction
            rb.linearVelocity = velocityDirection * spawnVelocity;
        }
    }
}