using UnityEngine;


public class AttractableObject : MonoBehaviour
{
    public Rigidbody2D _rb { get; private set; }
    private Transform _originalParentTransform;
    private SpringJoint2D _cachedJoint;
    private float _oldMass = 0f;
    void Start()
    {
        this._rb = GetComponent<Rigidbody2D>();
        
        // if (this._rb == null) return;
    }
    
    // Deactivate the Rigidbody2D
    public void DeactivateRigidbody()
    {
        if (this._rb == null) return;
        // Stops physics interactions but keeps transform updates
        this._oldMass = this._rb.mass;
        this._rb.mass = 1f;
    }

    // Reactivate the Rigidbody2D
    public void ReactivateRigidbody()
    {
        if (this._rb == null || this._oldMass == 0f) return;
        // Restores physics interactions
        this._rb.mass = this._oldMass;
    }
    
    // Attach a FixedJoint2D to the child object
    public void JoinToBodyFixed(Rigidbody2D newParent)
    {
        SpringJoint2D joint = this.gameObject.AddComponent<SpringJoint2D>();
        joint.breakForce = 200f;
        // joint.dampingRatio = 1f;
        joint.autoConfigureDistance = true;
        joint.connectedBody = newParent;
        this._cachedJoint = joint;
        
        if (this._rb == null) return;
        this._oldMass = this._rb.mass;
        this._rb.mass = 0.1f;
    }

    // Remove the SpringJoint2D to allow independent physics
    public void DetachBodyFixed()
    {
        if (this._cachedJoint == null) return;
        Destroy(this._cachedJoint);
        
        if (this._rb == null || this._oldMass == 0f) return;
        // Restores physics interactions
        this._rb.mass = this._oldMass;
    }
    
    public void SetParent(Transform p)
    {
        if (this._originalParentTransform == null)
        {
            this._originalParentTransform = this.transform.parent;
        }

        // this.GetComponent<Rigidbody2D>().linearVelocity = p.GetComponent<Rigidbody2D>().linearVelocity;
        this.transform.SetParent(p);
    }
    
    public void ResetParent()
    {
        this.transform.SetParent(this._originalParentTransform);
    }
}
