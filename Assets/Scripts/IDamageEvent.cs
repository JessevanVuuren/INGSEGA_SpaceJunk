using UnityEngine;

// Interface for classes which bundle data related to an event that attempts to damage something 
public interface IDamageEvent
{
    // Incoming damage. This is not necessarily the damage that actually ends up being dealt,
    // as the victim may still mitigate damage through resistances or other logic
    float IncomingDamage { get; }
    
    // The angle at which the impact took place, relative to the victim's transforms
    Vector2 LocalAngle { get; }
    
    // The location where the impact took place, relative to the victim's transforms
    Vector2 LocalPosition { get; }
}
