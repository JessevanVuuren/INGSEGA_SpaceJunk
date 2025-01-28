using UnityEngine;

namespace Projectiles
{
    public class MissileDamageEvent: IDamageEvent
    {
        public float IncomingDamage { get;}
        public Vector2? LocalAngle { get; }
        public Vector2? LocalPosition { get; }

        public MissileDamageEvent(float incomingDamage)
        {
            this.IncomingDamage = incomingDamage;
        }
    }
}
