using UnityEngine;

// Interface for classes which bundle data related to an event that attempts to damage something 
// Objects with this interface are meant to be passed to objects that implement the IDamageable interface
// `Script/CollisionDamageEvent` is an example of how this interface can be implemented
// `Script/GERV/Testing/DealDamageOnCollision` is an example of a script that creates a damage event
public interface IDamageEvent
{
    // Incoming damage. This is not necessarily the damage that actually ends up being dealt,
    // as the victim may still mitigate damage through resistances or other logic
    float IncomingDamage { get; }
    
    // The angle at which the impact took place, relative to the victim's transforms
    // Useful for when angled gits should do less damage than full-on hits
    // Optional, please do a null check before using
    Vector2? LocalAngle { get; }
    
    // The location where the impact took place, relative to the victim's transforms
    // Useful for when damage should be calculated differently based on the distance from victim's center
    // Optional, please do a null check before using
    Vector2? LocalPosition { get; }
}
