using UnityEngine;
using UnityEngine.Serialization;


public class AttractableObject : MonoBehaviour
{
    public float massModifierOnCapture = 0.01f;

    public bool IsValid { get; private set; }
    public bool IsCaptured { get; private set; }

    public Rigidbody2D Rb { get; private set; }
    
    private Transform _originalParentTransform;
    private SpringJoint2D _cachedJoint;
    private float _oldMass = 0f;
    void Start()
    {
        this.Rb = GetComponent<Rigidbody2D>();
        this.IsValid = this.Rb != null;        
    }
    
    public void SetStateCaptured()
    {
        if (this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = true;

        this._oldMass = this.Rb.mass;
        this.Rb.mass *= this.massModifierOnCapture;
    }

    public void UndoCapturedState()
    {
        if (!this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = true;
        
        this.Rb.mass = this._oldMass;
    }
}
