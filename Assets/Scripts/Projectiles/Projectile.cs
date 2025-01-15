using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("The lifespan of the projectile in seconds.")]
    public float lifespan = 5f; // Time before self-destruction
    public float damage = 50f;

    private void Start()
    {
        // Schedule the destruction of the projectile after the lifespan expires
        Destroy(gameObject, lifespan);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        
        if(damageable != null)
        {
            this.DealDamageTo(damageable);
        }
        
        // Destroy the projectile upon collision
        Destroy(gameObject);
    }

    private void DealDamageTo(IDamageable damageable)
    {
        IDamageEvent damageEvent = new MissileDamageEvent(this.damage);
        
        damageable.Damage(damageEvent);
    }
}