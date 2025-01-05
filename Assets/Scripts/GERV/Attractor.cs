using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Attractor : MonoBehaviour
{
    private const float MaxAttractionAngle = 180f;
    
    [Header("Attraction Settings")] 
    public Collider2D attractionCollider;
    public float attractionForce = 120f;  // Force of attraction
    public float dampeningForce = 200f;  // Force of attraction
    public float maxAttractionAngle = 45f; // Maximum angle (degrees) from the ship's forward direction

    [Header("Capture Settings")] 
    public Collider2D captureCollider;
    public float holdingForce = 30f;  // Force of attraction
    public float holdingDampeningForce = 60f;  // Force of attraction
    public uint maxCapturedObjects = 5;
    public float movementCompensationMod = 0.5f; // Modifier for position projection

    [Header("Input Settings")]
    public KeyCode activationKey = KeyCode.Mouse0; // Key to activate attraction

    private List<AttractableObject> _caughtObjects = new List<AttractableObject>();
    private List<AttractableObject> _attractedObjects = new List<AttractableObject>();

    private bool _setupValid = false;

    private Vector2 _previousPosition;
    // private Vector2 _previousVelocity;
    // private Vector2 _currentAcceleration;

    void Start()
    {
        this._setupValid = this.attractionCollider != null
                           && this.captureCollider != null
                           && this.attractionCollider.isTrigger
                           && this.maxAttractionAngle <= MaxAttractionAngle
                           && this.captureCollider.isTrigger;

        _previousPosition = transform.position;

        if (this._setupValid) return;

        Debug.LogError($"{nameof(Attractor)} has bad settings and won't work!");
    }

    void FixedUpdate()
    {
        if (!this._setupValid) return;

        HoldObjects(this._caughtObjects);

        if (Input.GetKey(activationKey))
        {
            AttractObjects(this._attractedObjects);
        }
        
        TrackPrevPosition();
    }

    private void TrackPrevPosition()
    {
        Vector2 currentPosition = transform.position;
        // Vector2 currentVelocity = (currentPosition - this._previousPosition) / Time.fixedDeltaTime;
        // this._currentAcceleration = (currentVelocity - this._previousVelocity) / Time.fixedDeltaTime;
        //
        // Cache for use in next frame
        // this._previousVelocity = currentVelocity;
        this._previousPosition = currentPosition;
    }

    private void HoldObjects(List<AttractableObject> attractables)
    {
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable.Rb;

            if (rb == null) continue;
            
            Vector2 posDelta = (Vector2)this.transform.position - this._previousPosition;
            Vector2 projectedPosition = (Vector2)this.transform.position + posDelta * movementCompensationMod;

            Vector2 attrVec = GetAttractionForce(rb, this.holdingForce, projectedPosition);
            Vector2 allowedDirection = attrVec.normalized;
            Vector2 dampVec = GetDampeningForce(rb, allowedDirection, this.holdingDampeningForce);

            // Vector2 compensationForce = _currentAcceleration * (rb.mass * movementCompensationMod);

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

            Vector2 attrVec = GetAttractionForce(rb, this.attractionForce, transform.position);
            Vector2 dampVec = GetDampeningForce(rb, attrVec.normalized, this.dampeningForce);

            rb.AddForce(attrVec + dampVec, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();

        if(attractable == null || !attractable.IsValid) return;

        bool captureAreaisFull = this._caughtObjects.Count >= this.maxCapturedObjects;
        if (!captureAreaisFull && other.IsTouching(this.captureCollider))
        {
            this._attractedObjects.Remove(attractable);
            this._caughtObjects.Add(attractable);
            attractable.SetStateCaptured();
        } else if (other.IsTouching(this.attractionCollider))
        {
            this._attractedObjects.Add(attractable);
        }
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
            this._caughtObjects.Remove(attractable);
            this._attractedObjects.Remove(attractable);
        }
    } 

    private Vector2 GetAttractionForce(Rigidbody2D rb, float magnitude, Vector2 targetPosition)
    {
        Vector2 directionToTarget = (rb.position - targetPosition).normalized;
        return -directionToTarget * (magnitude * Time.fixedDeltaTime);
    }   

    /// <summary>
    /// Applies a dampening force to cancel velocity components not aligned with the desired direction.
    /// </summary>
    /// <param name="otherRb">The RB of the object whose velocity that needs to be dampened.</param>
    /// <param name="allowedDirection">The desired movement direction.</param>
    /// <param name="magnitude">Strength of the new force</param>
    private Vector2 GetDampeningForce(Rigidbody2D otherRb, Vector2 allowedDirection, float magnitude)
    {
        Vector2 currentDirection = otherRb.linearVelocity.normalized;
        Vector2 dampeningDirection = allowedDirection - currentDirection;
        return dampeningDirection * (magnitude * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        float attractionRadius = this.attractionCollider.bounds.extents.x;
        Vector3 center = attractionCollider.bounds.center;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, attractionRadius);

        Gizmos.color = Color.green;
        Vector2 leftLimit = Quaternion.Euler(0, 0, -maxAttractionAngle) * transform.up;
        Vector2 rightLimit = Quaternion.Euler(0, 0, maxAttractionAngle) * transform.up;
        Gizmos.DrawLine(center, (Vector2)center + leftLimit * attractionRadius);
        Gizmos.DrawLine(center, (Vector2)center + rightLimit * attractionRadius);
    }
}
