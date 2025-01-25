using UnityEngine;

public class SpaceBoss : MonoBehaviour
{
    public GameObject[] aliens;
    public GameObject spaceWaist;
    public GameObject spaceWaistRandom;
    public float alienSpawnTime;
    public float waistSpawnTime;
    public HealthController healthController;
    public GameObject explosion;
    public float timeBetweenExp;
    public float expoRange;
    public float dieTime;
    public float randomGarbage = .5f;
    private float alienSpawnTimePriv;
    private float waistSpawnTimePriv;
    private float randomGarbagePriv;
    private bool deadAnim = false;
    private float dieTimePriv;

    void Start()
    {

    }

    void Update()
    {

        if (Time.time > alienSpawnTimePriv && aliens.Length > 0 && !deadAnim)
        {
            int alien = Random.Range(0, aliens.Length);
            Instantiate(aliens[alien], transform.position, Quaternion.identity);
            alienSpawnTimePriv = Time.time + alienSpawnTime;
        }

        if (Time.time > waistSpawnTimePriv && aliens.Length > 0 && !deadAnim)
        {
            Instantiate(spaceWaist, transform.position, Quaternion.identity);
            waistSpawnTimePriv = Time.time + waistSpawnTime;
        }

        if (Time.time > randomGarbagePriv && aliens.Length > 0 && !deadAnim)
        {
            Instantiate(spaceWaistRandom, transform.position, Quaternion.identity);
            randomGarbagePriv = Time.time + randomGarbage;
        }


        if (healthController.healthBar.value <= 0)
        {
            deadAnim = true;
            if (Time.time > timeBetweenExp)
            {
                timeBetweenExp = Time.time + timeBetweenExp;
            }

            float angle = Random.Range(0, 2 * Mathf.PI);
            float x = Mathf.Cos(angle) * Random.Range(0, expoRange);
            float y = Mathf.Sin(angle) * Random.Range(0, expoRange);

            Vector3 pos = new Vector3(x, y, 0) + transform.position;

            Instantiate(explosion, pos, Quaternion.identity);

            if (Time.time > dieTimePriv + dieTime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            dieTimePriv = Time.time;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, expoRange);
    }

}
