using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AttractableObject : MonoBehaviour, ICollectable
{
    public String captureLayerName = "CapturedObjects";

    public bool IsValid { get; private set; }
    public bool IsCaptured { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private int _captureLayer;
    private int _originalLayer;

    void Start()
    {
        this.Rb = GetComponent<Rigidbody2D>();
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this.IsValid = this.Rb != null && this._spriteRenderer != null;
        
        if (!this.IsValid) return;
        
        this._captureLayer = LayerMask.NameToLayer(this.captureLayerName);
        this._originalLayer = this.gameObject.layer;
    }

    public void SetStateCaptured()
    {
        if (this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = true;

        this.gameObject.layer = this._captureLayer;

        // Change the object's color to red
        this._spriteRenderer.color = Color.red;
    }

    public void UndoCapturedState()
    {
        if (!this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = false;

        this.gameObject.layer = this._originalLayer;

        // Reset the object's color to white
        this._spriteRenderer.color = Color.white;
    }

    // This is when the object is collected by the mothership.
    // It's NOT the same as capturing/catching, which simply involves holding the object with the Mass Collector
    public void Collect()
    {
        Destroy(this.gameObject);
    }
}