using UnityEngine;

public class SimpleWaveSpawner : MonoBehaviour
{
    public GameObject[] aliens;
    public float randomSpawnRange = 1;
    
    public float timeBetweenSpawns;
    private float nextSpawn;

    void Update()
    {
        if (Time.time > nextSpawn && aliens.Length > 0)
        {
            int alien = Random.Range(0, aliens.Length);

            float angle = Random.Range(0, 2 * Mathf.PI);
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            Vector3 loc = new Vector3(x, y, 0) * randomSpawnRange;

            Instantiate(aliens[alien], loc, Quaternion.identity);
            nextSpawn = Time.time + timeBetweenSpawns;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, randomSpawnRange);
    }
}
