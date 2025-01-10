using System;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public Collider2D CollectionCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool isValid = !(this.CollectionCollider == null || !this.CollectionCollider.isTrigger);

        if (isValid) return;

        this.enabled = false;
        
        Debug.LogError($"Collector {nameof(this.gameObject)} isn't working! Invalid settings.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ICollectable collectable = other.GetComponent<ICollectable>();

        if (collectable == null) return;
        
        Debug.Log($"Collected! {nameof(collectable)}");
        
        collectable.Collect();
    }
}
