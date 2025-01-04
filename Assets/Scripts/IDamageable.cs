using UnityEngine;

// Interface which lets objects take damage
// And example implementation of this interface can be found in `Script/HealthController`
public interface IDamageable
{
    public void Damage(IDamageEvent damageEvent);
}
    