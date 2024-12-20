using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnCollision : MonoBehaviour
{
    private const int MaxContactCount = 3;

    [Header("Damage dealing Settings")]
    public float minImpulse = 0.1f;
    public float damageAtMinImpulse = 5f;
    public bool dontDealDamageBelowMinImpulse = false;
    public float maxImpulse = 5f;
    public float damageAtMaxImpulse = 100f;
    
    // [Header("Damage to self settings")]

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the ICollisionDamageable component from the other object
        ICollisionDamageable other = collision.gameObject.GetComponent<ICollisionDamageable>();
        if (other == null) return;

        // Calculate the total impulse from the collision
        float impulse = GetImpulse(collision);

        // Skip dealing damage if impulse is below the minimum threshold and flag is set
        if (dontDealDamageBelowMinImpulse && impulse < minImpulse) return;

        // Calculate the interpolated damage
        float dmg = InterpolateDamage(impulse);

        // Debugging damage value
        Debug.Log($"Impulse: {impulse:F2}, Damage: {dmg:F2}, Point: {GetCollisionPoint(collision)}, angle: {GetCollisionAngle(collision)}");

        // Deal damage to the other object
        other.DealCollisionDamage(dmg);

        // Destroy this object after the collision
        Destroy(this.gameObject);
    }

    private float InterpolateDamage(float impulse)
    {
        // If impulse is below the minimum threshold, return minimum damage
        if (impulse <= minImpulse) return damageAtMinImpulse;

        // If impulse is above or equal to the maximum threshold, return maximum damage
        if (impulse >= maxImpulse) return damageAtMaxImpulse;

        // Perform linear interpolation for impulses between minImpulse and maxImpulse
        return Mathf.Lerp(damageAtMinImpulse, damageAtMaxImpulse, (impulse - minImpulse) / (maxImpulse - minImpulse));
    }

    private float GetImpulse(Collision2D collision)
    {
        // Collect all contact points
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collision.GetContacts(contacts);

        // Limit the number of iterations to MaxContactCount
        int iterations = Mathf.Min(contacts.Count, MaxContactCount);

        // Sum up the normal impulses from the contacts
        float totalImpulse = 0f;
        for (int i = 0; i < iterations; i++)
        {
            totalImpulse += contacts[i].normalImpulse;
        }

        return totalImpulse;
    }
    
    private float GetCollisionAngle(Collision2D collision)
    {
        // Get the first contact point
        ContactPoint2D contact = collision.GetContact(0);

        // Get the contact normal (surface orientation at the collision point)
        // Get the relative velocity (direction of motion at impact)
        // Normalize the vectors (important for accurate angle calculation)
        Vector2 relativeDirection = contact.normal.normalized;
        Vector2 normalizedNormal = collision.relativeVelocity.normalized;

        // Calculate the angle using the dot product
        float dotProduct = Vector2.Dot(relativeDirection, normalizedNormal);

        // Ensure dotProduct is within valid range to avoid NaN
        dotProduct = Mathf.Clamp(dotProduct, -1f, 1f);

        // Compute the angle in degrees (0 = perpendicular, 90 = parallel)
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angle;
    }

    private Vector2 GetCollisionPoint(Collision2D collision)
    {
        // Get the collision point in world space
        Vector2 worldPoint = collision.GetContact(0).point;

        // Convert the world point to the collidee's local space
        Vector2 localPoint = collision.transform.InverseTransformPoint(worldPoint);

        return localPoint;
    }
}
