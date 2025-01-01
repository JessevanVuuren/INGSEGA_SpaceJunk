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
    public float holdingForce = 30f;  // Force of attraction
    public float holdingDampeningForce = 60f;  // Force of attraction

    [Header("Input Settings")]
    public KeyCode activationKey = KeyCode.Mouse0; // Key to activate attraction
    // public KeyCode releaseKey = KeyCode.Mouse1; // Key to activate attraction

    private List<AttractableObject> _caughtObjects = new List<AttractableObject>();
    private List<AttractableObject> _attractedObjects = new List<AttractableObject>();

    private bool _setupValid = false;
    
    void Start()
    {
        this._setupValid = this.attractionCollider != null
                           && this.captureCollider != null
                           && this.attractionCollider.isTrigger
                           && this.attractionCollider.bounds.Contains(this.transform.position)
                           && this.maxAttractionAngle <= 180f
                           && this.captureCollider.isTrigger
                           && this.captureCollider.bounds.Contains(this.transform.position);
        
        if (this._setupValid) return;
        
        Debug.LogError($"{nameof(Attractor)} has bad settings and won't work!");
    }

    void Update()
    {
        if (!this._setupValid) return;
        
        HoldObjects(this._caughtObjects); // This is where the code tries to calculate ship velocity delta 
        
        if (Input.GetKey(activationKey))
        {
            AttractObjects(this._attractedObjects);
        }
    }
    
    private void HoldObjects(List<AttractableObject> attractables)
    {
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable.Rb;
        
            if (rb == null) continue;
            
            Vector2 attrVec = GetAttractionForce(rb, this.holdingForce);
            Vector2 allowedDirection = attrVec.normalized;
            Vector2 dampVec = GetDampeningForce(rb, allowedDirection, this.holdingDampeningForce);
            
            // Debug.Log($"Attr: {attrVec} Damp: {dampVec} Sum: {attrVec + dampVec}");
            
            rb.AddForce(attrVec + dampVec, ForceMode2D.Force);
        }
    }

    private void AttractObjects(List<AttractableObject> attractables)
    {
        
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable.Rb;

            if (rb == null) continue;
            
            Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToTarget);
            
            if (angle > maxAttractionAngle) continue;
            
            Vector2 attrVec = GetAttractionForce(rb, this.attractionForce);
            Vector2 dampVec = GetDampeningForce(rb, attrVec.normalized, this.dampeningForce);
            
            // Debug.Log($"Attr: {attrVec} Damp: {dampVec} Sum: {attrVec + dampVec}");
            
            rb.AddForce(attrVec + dampVec, ForceMode2D.Force);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable == null || !attractable.IsValid) return;

        if (other.IsTouching(this.captureCollider))
        {
            this._attractedObjects.Remove(attractable);
            this._caughtObjects.Add(attractable);
            attractable.SetStateCaptured();
        } else if (other.IsTouching(this.attractionCollider))
        {
            this._attractedObjects.Add(attractable);
        }
        // Debug.Log($"{this._attractedObjects.Count} {this._caughtObjects.Count}");
    } 
    
    private void OnTriggerExit2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable == null || !attractable.IsValid) return;

        if (!other.IsTouching(this.captureCollider) && other.IsTouching(this.attractionCollider))
        {
            this._caughtObjects.Remove(attractable);
            attractable.UndoCapturedState();
            this._attractedObjects.Add(attractable);
        } else
        {
            this._attractedObjects.Remove(attractable);
        }
        // Debug.Log($"{this._attractedObjects.Count} {this._caughtObjects.Count}");
    } 
    
    private Vector2 GetAttractionForce(Rigidbody2D rb, float magnitude)
    {
        Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;

        // Vector2 direction = ((Vector2)transform.position + (Vector2)transform.up * 2f - rb.position).normalized;
        // rb.AddForce(-directionToTarget * attractionForce * Time.deltaTime, ForceMode2D.Force);

        return -directionToTarget * (magnitude * Time.deltaTime);
    }

    /// <summary>
    /// Applies a dampening force to cancel velocity components not aligned with the desired direction.
    /// </summary>
    /// <param name="otherRb">The RB of the object whose velocity that needs to be dampened.</param>
    /// <param name="allowedDirection">The desired movement direction.</param>
    /// <param name="magnitude">Strength of the new force</param>
    private Vector2 GetDampeningForce(Rigidbody2D otherRb, Vector2 allowedDirection, float magnitude)
    {
        // Project current velocity onto the input direction
        Vector2 currentDirection = otherRb.linearVelocity.normalized;
        
        Vector2 dampeningDirection = allowedDirection - currentDirection;
        
        // Calculate the velocity that needs to be canceled
        return dampeningDirection * (magnitude * Time.deltaTime);
    }
    
    // private void ReleaseObjects(Rigidbody2D shipRigidbody)
    // {
    //     foreach (var rb in attractedObjects)
    //     {
    //         if (rb == null) continue;
    //
    //         // Add ship's velocity change to the attracted object's velocity
    //         rb.velocity += (shipRigidbody.velocity - _previousPosition);
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
 