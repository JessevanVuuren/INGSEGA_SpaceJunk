using UnityEngine;
using System.Linq;
using UnityEditor;

public class Enemy : MonoBehaviour
{

    public float dmg = 50;
    private Rigidbody2D rb;
    public float speed = 0;
    public float maxLookRadius = 5;
    public GameObject explosion;
    public bool isShoot = false;
    public float timeBetweenShoot = 5;
    public GameObject missilePrefab;
    public Transform launchPoint;
    public float launchForce = 10f;
    private float timing = 0;
    private Vector3 playerPos;
    public GameObject player;

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

        if (player != null && Vector3.Distance(transform.position, playerPos) < maxLookRadius)
        {

            rb.MovePosition(Vector3.MoveTowards(transform.position, playerPos, speed * Time.deltaTime));

            Vector3 direction = playerPos - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y);
            rb.MoveRotation(-(angle * Mathf.Rad2Deg));

            
            if (isShoot && Time.time > timing)
            {
                Shoot();
                timing = timeBetweenShoot + Time.time;
            }
        }
    }

    public void Shoot()
    {
        GameObject missile = Instantiate(missilePrefab, launchPoint.position, launchPoint.rotation);
        Rigidbody2D missileRb = missile.GetComponent<Rigidbody2D>();

        if (missileRb != null)
        {
            missileRb.linearVelocity = launchPoint.up * launchForce;
        }
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable other = collision.gameObject.GetComponent<IDamageable>();
        if (other == null) return;

        CollisionDamageEvent dmgEvent = new CollisionDamageEvent(dmg);

        other.Damage(dmgEvent);

        Explode();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxLookRadius);
    }
}
