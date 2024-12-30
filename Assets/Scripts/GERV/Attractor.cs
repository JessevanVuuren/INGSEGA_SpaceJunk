using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Attractor : MonoBehaviour
{
    [Header("Attraction Settings")] 
    public Collider2D attractionCollider;
    public float attractionForce = 120f;  // Force of attraction
    public float dampeningForce = 200f;  // Force of attraction
    public float maxAttractionAngle = 45f; // Maximum angle (degrees) from the ship's forward direction

    [Header("Capture Settings")] 
    // public float captureRange = 5f;
    public Collider2D captureCollider;
    public Rigidbody2D parentRB;

    [Header("Input Settings")]
    public KeyCode activationKey = KeyCode.Mouse0; // Key to activate attraction
    // public KeyCode releaseKey = KeyCode.Mouse1; // Key to activate attraction

    private List<AttractableObject> _caughtObjects = new List<AttractableObject>();
    private List<AttractableObject> _attractedObjects = new List<AttractableObject>();
    private Vector2 _previousShipVelocity = new Vector2();

    private bool _setupValid = false;
    
    void Start()
    {
        this.parentRB = this.GetComponentInParent<Rigidbody2D>();

        this._setupValid = this.attractionCollider != null
                          && this.captureCollider != null
                          && this.attractionCollider.isTrigger
                          && this.attractionCollider.bounds.Contains(this.transform.position)
                          && this.maxAttractionAngle <= 180f
                          && this.captureCollider.isTrigger
                          && this.captureCollider.bounds.Contains(this.transform.position)
                          && this.parentRB != null;
        
        if (this._setupValid) return;
        
        Debug.LogError($"{nameof(Attractor)} has bad settings and won't work!");
    }

    void Update()
    {
        if (!this._setupValid) return;
        
        HoldObjects(this._caughtObjects);
        
        if (Input.GetKey(activationKey))
        {
            AttractObjects(this._attractedObjects);
        }

        _previousShipVelocity = this.parentRB.linearVelocity;
    }
    
    private void HoldObjects(List<AttractableObject> attractables)
    {
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable._rb;
        
            if (rb == null) continue;
            
            // Vector2 parentMovement = this.GetComponentInParent<Rigidbody2D>().linearVelocity * Time.deltaTime;
            // rb.linearVelocity = this.GetComponentInParent<Rigidbody2D>().linearVelocity;
            
            Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToTarget);
            
            Vector2 attractionForce = GetAttractionForce(rb);
            Vector2 dampeningDirection = (attractionForce + this.GetGERVVelocityDelta()).normalized;
            Vector2 dampeningForce = GetDampeningForce(rb, dampeningDirection);
            
            // Debug.Log($"Attr: {attractionForce} Damp: {dampeningForce} Sum: {attractionForce + dampeningForce}");
            
            rb.AddForce(attractionForce + dampeningForce * 2f, ForceMode2D.Force);
            // rb.AddForce(attractionForce * 2f, ForceMode2D.Force);
            // Vector2 additionalSpeed = this.GetGERVVelocityDelta();
            // rb.linearVelocity += additionalSpeed;
        }
    }

    private Vector2 GetGERVVelocityDelta()
    {
        Debug.Log($"Curr Velo {this.parentRB.linearVelocity} new Velo {this._previousShipVelocity}");
        return this.parentRB.linearVelocity - this._previousShipVelocity;
    }

    private void AttractObjects(List<AttractableObject> attractables)
    {
        
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable._rb;

            if (rb == null) continue;
            
            Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToTarget);
            
            if (angle > maxAttractionAngle) continue;
            
            Vector2 attractionForce = GetAttractionForce(rb);
            Vector2 dampeningForce = GetDampeningForce(rb, attractionForce.normalized);
            
            // Debug.Log($"Attr: {attractionForce} Damp: {dampeningForce} Sum: {attractionForce + dampeningForce}");
            
            rb.AddForce(attractionForce + dampeningForce, ForceMode2D.Force);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable == null || attractable._rb == null) return;
        // if(this._attractedObjects.Contains(attractable) || attractable == null || attractable._rb == null) return;

        if (other.IsTouching(this.captureCollider))
        {
            this._attractedObjects.Remove(attractable);
            this._caughtObjects.Add(attractable);
            // attractable.SetParent(this.transform);
            // attractable.JoinToBodyFixed(this.GetComponentInParent<Rigidbody2D>());
            attractable.DeactivateRigidbody();
        } else if (other.IsTouching(this.attractionCollider))
        {
            this._attractedObjects.Add(attractable);
        }
        // Debug.Log($"{this._attractedObjects.Count} {this._caughtObjects.Count}");
    } 
    
    private void OnTriggerExit2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable == null || attractable._rb == null) return;

        if (!other.IsTouching(this.captureCollider) && other.IsTouching(this.attractionCollider))
        {
            this._caughtObjects.Remove(attractable);
            // attractable.ResetParent();
            // attractable.DetachBodyFixed();
            attractable.ReactivateRigidbody();
            this._attractedObjects.Add(attractable);
        } else
        {
            this._attractedObjects.Remove(attractable);
        }
        // Debug.Log($"{this._attractedObjects.Count} {this._caughtObjects.Count}");
    } 

    private Vector2 GetAttractionForce(Rigidbody2D rb)
    {
        Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;

        // Vector2 direction = ((Vector2)transform.position + (Vector2)transform.up * 2f - rb.position).normalized;
        // rb.AddForce(-directionToTarget * attractionForce * Time.deltaTime, ForceMode2D.Force);

        return -directionToTarget * (attractionForce * Time.deltaTime);
    }

    /// <summary>
    /// Applies a dampening force to cancel velocity components not aligned with the desired direction.
    /// </summary>
    /// <param name="otherRb">The RB of the object whose velocity that needs to be dampened.</param>
    /// <param name="attractionDirection">The desired movement direction.</param>
    private Vector2 GetDampeningForce(Rigidbody2D otherRb, Vector2 attractionDirection)
    {
        // Project current velocity onto the input direction
        Vector2 currentDirection = otherRb.linearVelocity.normalized;
        
        Vector2 dampeningDirection = attractionDirection - currentDirection;
        
        // Calculate the velocity that needs to be canceled
        return dampeningDirection * (this.dampeningForce * Time.deltaTime);
    }
    
    // private void ReleaseObjects(Rigidbody2D shipRigidbody)
    // {
    //     foreach (var rb in attractedObjects)
    //     {
    //         if (rb == null) continue;
    //
    //         // Add ship's velocity change to the attracted object's velocity
    //         rb.velocity += (shipRigidbody.velocity - _previousShipVelocity);
    //     }
    //
    //     attractedObjects.Clear();
    // }

    private void OnDrawGizmosSelected()
    {
        float attractionRadius = this.attractionCollider.bounds.extents.x;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    
        Gizmos.color = Color.green;
        Vector2 leftLimit = Quaternion.Euler(0, 0, -maxAttractionAngle) * transform.up;
        Vector2 rightLimit = Quaternion.Euler(0, 0, maxAttractionAngle) * transform.up;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftLimit * attractionRadius);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightLimit * attractionRadius);
    }
}
 