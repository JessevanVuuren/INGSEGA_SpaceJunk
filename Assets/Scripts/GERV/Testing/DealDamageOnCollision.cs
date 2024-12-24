using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnCollision : MonoBehaviour
{
    [Header("Damage dealing Settings")]
    public ImpulseInterpolationDamageCalculator dmgCalculator;
    // [Header("Damage to self settings")]

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the IDamageable component from the other object
        IDamageable other = collision.gameObject.GetComponent<IDamageable>();
        if (other == null) return;

        // Calculate the total impulse from the collision
        float impulse = CollisionDamageEvent.Impulse(collision);

        // Skip dealing damage if impulse is below the minimum threshold and flag is set
        if (dmgCalculator.dontDealDamageBelowMinImpulse && impulse < dmgCalculator.minImpulse) return;

        // Calculate the interpolated damage
        float dmg = dmgCalculator.GetDamage(impulse);

        CollisionDamageEvent dmgEvent = new CollisionDamageEvent(
            dmg, 
            CollisionDamageEvent.CollisionAngleLocal(collision), 
            CollisionDamageEvent.CollisionPoint(collision)
            );

        // Debugging damage value
        Debug.Log($"Impulse: {impulse:F2}, Damage: {dmgEvent.IncomingDamage}, Point: {dmgEvent.LocalPosition}, angle: {dmgEvent.LocalAngle}");

        // Deal damage to the other object
        other.Damage(dmgEvent);

        // Destroy this object after the collision
        // Destroy(this.gameObject);
    }
}
