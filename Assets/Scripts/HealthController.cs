using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, IDamageable
{
    public float Health = 100f;
    // public Slider healthBar;
    // public GameObject onDestructionEffect;

    // private void Start()
    // {
    //     if (this.healthBar == null) return;
    //     this.healthBar.maxValue = Health;
    //     this.healthBar.value = Health;
    // }

    public void Damage(IDamageEvent damageEvent)
    {
        this.Health -= damageEvent.IncomingDamage;
        
        // Debug.Log($"{nameof(This)} took {damageEvent.IncomingDamage} damage. Remaining: {this.Health}");

        // if (this.healthBar)
        // {
        //     this.healthBar.value = Health;
        // }

        if (this.Health > 0) return;

        this.DestructSelf();
    }

    private void DestructSelf()
    {
        // Instantiate(onDestructionEffect, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }
}
