using UnityEngine;

public class AlienForShadow : MonoBehaviour
{
    public float speed;
    public float exponentially = 0.0f;
    public GameObject player;


    private Rigidbody2D rb;
    private Vector3 originalPos;
    private Vector3 endPos;
    void Start()
    {
        player = GameObject.FindWithTag("MainCamera");

        rb = GetComponent<Rigidbody2D>();

        originalPos = transform.position;

        endPos = originalPos;
        endPos.x += 15;
        endPos.y -= 12;
    }

    void FixedUpdate()
    {
        Vector3 playerPos = player.transform.position;
        speed += exponentially;
        rb.MovePosition(Vector3.MoveTowards(transform.position, endPos + playerPos, speed * Time.deltaTime));

        Vector3 direction = playerPos - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y);
        rb.MoveRotation(-(angle * Mathf.Rad2Deg));

        Vector3 newPos = transform.position - playerPos;
        if (newPos.y < -10 && newPos.x > 5) Destroy(gameObject);
    }
}
