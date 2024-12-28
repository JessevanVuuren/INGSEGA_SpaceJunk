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

    [Header("Input Settings")]
    public KeyCode activationKey = KeyCode.Mouse0; // Key to activate attraction
    // public KeyCode releaseKey = KeyCode.Mouse1; // Key to activate attraction

    private List<AttractableObject> _caughtObjects = new List<AttractableObject>();
    // private Vector2 previousShipVelocity;

    private bool setupValid = false;

    void Start()
    {
        this.setupValid = this.attractionCollider != null 
                          && this.attractionCollider.isTrigger 
                          && this.attractionCollider.bounds.Contains(this.transform.position)
                          && this.maxAttractionAngle < 360f;
        
        if (this.setupValid) return;
        
        Debug.LogError($"{nameof(Attractor)} has bad settings and won't work!");
    }

    void Update()
    {
        if (!this.setupValid) return;
        
        if (Input.GetKey(activationKey))
        {
            List<Collider2D> otherColliders = new List<Collider2D>(); 
        
            // TODO: consider adding  ContactFilter2D contactFilter param to grab objects more precisely?
            Physics2D.OverlapCollider(this.attractionCollider, otherColliders);

            List<AttractableObject> attractables = otherColliders
                .Select(collider => collider.GetComponent<AttractableObject>())
                .Where(attractable => attractable != null)
                .ToList();
            
            AttractObjects(attractables);
        }
        // else if (Input.GetKey(releaseKey))
        // {
        //     ReleaseObjects();
        // }

        // previousShipVelocity = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
    }

    private void AttractObjects(List<AttractableObject> attractables)
    {
        
        foreach (var attractable in attractables)
        {
            Rigidbody2D rb = attractable._rb;

            if (rb == null) return;
            
            Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToTarget);
            
            if (angle > maxAttractionAngle) return;
            
            Vector2 attractionForce = GetAttractionForce(rb);
            Vector2 dampeningForce = GetDampeningForce(rb, attractionForce.normalized);
            
            // Debug.Log($"Attr: {attractionForce} Damp: {dampeningForce} Sum: {attractionForce + dampeningForce}");
            
            rb.AddForce(attractionForce + dampeningForce, ForceMode2D.Force);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable==null) return;

        this._caughtObjects.Add(attractable);
    } 
    
    private void OnTriggerExit2D(Collider2D other)
    {
        AttractableObject attractable = other.GetComponent<AttractableObject>();
        
        if(attractable==null) return;

        this._caughtObjects.Remove(attractable);
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
    //         rb.velocity += (shipRigidbody.velocity - previousShipVelocity);
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
 