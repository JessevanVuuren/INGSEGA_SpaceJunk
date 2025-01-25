using UnityEngine;

public class EnemyGarbage : MonoBehaviour
{

    public float dmg = 50;
    private Rigidbody2D rb;
    public float speed = 0;
    public float rotateSpeed = 0;
    public GameObject explosion;
    public float timeBetweenShoot = 5;
    public GameObject garbage;
    public float minRadius = 5;
    public Transform launchPoint;
    private float timing = 0;
    private Vector3 playerPos;
    public GameObject player;
    private float angleFromOrigin = 0;
    private bool inPlayerRadius = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            return;
        }


        playerPos = player.transform.position;

    }
    void FixedUpdate()
    {
        if (player == null) return;

        if (Vector3.Distance(transform.position, playerPos) < minRadius) inPlayerRadius = true;
        if (Vector3.Distance(transform.position, playerPos) > minRadius && !inPlayerRadius)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, playerPos, speed * Time.deltaTime));
        }
        else
        {
            angleFromOrigin += rotateSpeed * 0.01f;

            float x = Mathf.Cos(angleFromOrigin) * minRadius;
            float y = Mathf.Sin(angleFromOrigin) * minRadius;

            rb.MovePosition(new Vector3(x, y, 0) + playerPos);
        }


        Vector3 direction = playerPos - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y);
        rb.MoveRotation(-(angle * Mathf.Rad2Deg));

        if (Time.time > timing)
        {
            Instantiate(garbage, launchPoint.position, launchPoint.rotation);
            timing = timeBetweenShoot + Time.time;

        }
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRadius);
    }
}
