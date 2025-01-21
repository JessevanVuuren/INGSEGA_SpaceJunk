using UnityEngine;

public class SimpleWaveSpawner : MonoBehaviour
{
    public GameObject alien;
    public Transform[] spawnLocations;
    public float timeBetweenSpawns;
    private float nextSpawn;

    void Update()
    {
        if (Time.time > nextSpawn && alien != null)
        {
            int loc = Random.Range(0, spawnLocations.Length - 1);
            Instantiate(alien, spawnLocations[loc].position, Quaternion.identity);
            nextSpawn = Time.time + timeBetweenSpawns;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            Gizmos.DrawWireSphere(spawnLocations[i].position, .2f);
        }
    }
}
