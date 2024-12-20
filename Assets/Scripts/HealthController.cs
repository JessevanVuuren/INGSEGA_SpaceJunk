using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, ICollisionDamageable
{
    public float Health {
        get;
        set;
    } = 100f;
    // public Slider healthBar;
    // public GameObject onDestructionEffect;

    // private void Start()
    // {
    //     if (this.healthBar == null) return;
    //     this.healthBar.maxValue = Health;
    //     this.healthBar.value = Health;
    // }

    public void DealCollisionDamage(float damage)
    {
        this.Health -= damage;

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
