using UnityEngine;

namespace GERV.Testing
{
    [System.Serializable]
    public class ImpulseInterpolationDamageCalculator
    {
        public float minImpulse = 0.1f;
        public float damageAtMinImpulse = 5f;
        public bool dontDealDamageBelowMinImpulse = false;
        public float maxImpulse = 5f;
        public float damageAtMaxImpulse = 100f;
    
        /// <summary>
        /// Interpolates the damage based on the given impulse.
        /// </summary>
        /// <param name="impulse">The collision impulse.</param>
        /// <returns>The interpolated damage value.</returns>
        public float GetDamage(float impulse)
        {
            // If impulse is below the minimum threshold, return minimum damage
            if (impulse <= minImpulse) return damageAtMinImpulse;

            // If impulse is above or equal to the maximum threshold, return maximum damage
            if (impulse >= maxImpulse) return damageAtMaxImpulse;

            // Perform linear interpolation for impulses between minImpulse and maxImpulse
            return Mathf.Lerp(damageAtMinImpulse, damageAtMaxImpulse, (impulse - minImpulse) / (maxImpulse - minImpulse));
        }
    }
}
