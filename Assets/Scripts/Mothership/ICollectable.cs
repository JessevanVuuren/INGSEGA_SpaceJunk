using System;
using UnityEngine;

public interface ICollectable
{
    // TODO: change returntype to something which renpresents the type of collectible.
    public void Collect();
    
    public String GetTag();
    
    public IDamageEvent GetDamage();
}
