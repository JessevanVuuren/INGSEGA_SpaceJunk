using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for handling common collision-related calculations.
/// Provides methods for reusability and convenience in various scripts.
/// </summary>
public class CollisionDamageEvent : IDamageEvent
{
    private static readonly int _maxContactCount = 3;

    public float IncomingDamage { get; private set; }
    public Vector2? LocalAngle { get; private set; }
    public Vector2? LocalPosition { get; private set; }

    public CollisionDamageEvent(float incomingDamage, Vector2 localAngle, Vector2 localPosition)
    {
        IncomingDamage = incomingDamage;
        LocalAngle = localAngle;
        LocalPosition = localPosition;
    }
    
    /// <summary>
    /// Calculates the total impulse of a collision based on the contact points.
    /// Useful for estimating collision intensity for damage calculations.
    /// </summary>
    public static float Impulse(Collision2D collision)
    {
        // Collect all contact points
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collision.GetContacts(contacts);

        // Limit the number of iterations to _maxContactCount
        int iterations = Mathf.Min(contacts.Count, _maxContactCount);

        // Sum up the normal impulses from the contacts
        float totalImpulse = 0f;
        for (int i = 0; i < iterations; i++)
        {
            totalImpulse += contacts[i].normalImpulse;
        }

        return totalImpulse;
    }
    
    /// <summary>
    /// Calculates the impact angle relative to the surface normal of the collider.
    /// Returns the angle in degrees (0 = perpendicular, 90 = parallel).
    /// </summary>
    public static float CollisionAngle(Collision2D collision)
    {
        // Get the first contact point
        ContactPoint2D contact = collision.GetContact(0);

        // Get the contact normal (surface orientation at the collision point)
        // Get the relative velocity (direction of motion at impact)
        // Normalize the vectors (important for accurate angle calculation)
        Vector2 normalizedNormal = contact.normal.normalized;
        Vector2 relativeDirection = collision.relativeVelocity.normalized;

        // Calculate the angle using the dot product
        float dotProduct = Vector2.Dot(relativeDirection, normalizedNormal);

        // Ensure dotProduct is within valid range to avoid NaN
        dotProduct = Mathf.Clamp(dotProduct, -1f, 1f);

        // Compute the angle in degrees (0 = perpendicular, 90 = parallel)
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angle;
    }
    
    /// <summary>
    /// Calculates the collision angle relative to the local rotation of the collidee.
    /// Returns a normalized 2D vector representing the direction of impact.
    /// </summary>
    public static Vector2 CollisionAngleLocal(Collision2D collision)
    {
        // Get the relative velocity (direction of motion at impact)
        Vector2 relativeVelocity = collision.relativeVelocity;

        // Transform the relative velocity into the collidee's local space
        Transform collideeTransform = collision.transform;
        Vector2 localVelocity = collideeTransform.InverseTransformDirection(relativeVelocity);

        // Normalize the resulting vector to ensure it's a unit vector
        return localVelocity.normalized;
    }

    /// <summary>
    /// Calculates the collision point relative to the collidee's local coordinate space.
    /// Useful for determining the exact location of impact on the collidee.
    /// </summary>
    public static Vector2 CollisionPoint(Collision2D collision)
    {
        // Get the collision point in world space
        Vector2 worldPoint = collision.GetContact(0).point;

        // Convert the world point to the collidee's local space
        Vector2 localPoint = collision.transform.InverseTransformPoint(worldPoint);

        return localPoint;
    }
}
