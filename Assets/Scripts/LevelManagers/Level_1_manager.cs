using UnityEngine;

public class Level_1_manager : MonoBehaviour
{

    public int delayTimeForAlien = 10;
    public GameObject alien;
    public GameObject player;

    private float alienTime;
    private bool alienDidSpawn = false;

    void Start()
    {
        alienTime = Time.time + delayTimeForAlien;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > alienTime && !alienDidSpawn)
        {
            alienDidSpawn = true;
            
            Vector3 spawn = player.transform.position;
            spawn.x =- 9;

            Instantiate(alien, spawn, Quaternion.identity);
        }
    }
}
