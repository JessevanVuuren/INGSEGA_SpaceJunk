using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AttractableObject : MonoBehaviour
{
    public float massModifierOnCapture = 0.01f;
    public String captureLayerName = "CapturedObjects";

    public bool IsValid { get; private set; }
    public bool IsCaptured { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private float _originalMass = 0f;
    private int _captureLayer;
    private int _originalLayer;

    void Start()
    {
        this.Rb = GetComponent<Rigidbody2D>();
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this.IsValid = this.Rb != null && this._spriteRenderer != null;
        
        if (!this.IsValid) return;
        
        this._originalMass = this.Rb.mass;
        this._captureLayer = LayerMask.NameToLayer(this.captureLayerName);
        this._originalLayer = this.gameObject.layer;
    }

    public void SetStateCaptured()
    {
        if (this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = true;

        this.Rb.mass *= this.massModifierOnCapture;
        this.gameObject.layer = this._captureLayer;

        // Change the object's color to red
        this._spriteRenderer.color = Color.red;
    }

    public void UndoCapturedState()
    {
        if (!this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = false;

        this.Rb.mass = this._originalMass;
        this.gameObject.layer = this._originalLayer;

        // Reset the object's color to white
        this._spriteRenderer.color = Color.white;
    }
}