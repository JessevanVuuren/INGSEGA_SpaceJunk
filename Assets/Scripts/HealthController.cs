using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, IDamageable
{
    public float Health = 100f;
    [CanBeNull] public Slider healthBar;
    public bool followTransform;
    public Vector3 offset;
    // public GameObject onDestructionEffect;

    private void Start()
    {
        if (this.healthBar == null) return;
        this.healthBar.maxValue = Health;
        this.healthBar.value = Health;
    }

    public void Damage(IDamageEvent damageEvent)
    {
        this.Health -= damageEvent.IncomingDamage;
        if (followTransform) healthBar.gameObject.SetActive(true);
        // Debug.Log($"{nameof(This)} took {damageEvent.IncomingDamage} damage. Remaining: {this.Health}");
        if (this.healthBar)
        {
            this.healthBar.value = Health;
        }

        if (this.Health > 0) return;

        this.DestructSelf();    
    }

    public void FixedUpdate() {
        if (followTransform) {

            healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + offset);
        }
    }

    private void DestructSelf()
    {
        // Instantiate(onDestructionEffect, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }
}
