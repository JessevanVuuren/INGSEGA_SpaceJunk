using System.Collections.Generic;
using UnityEngine;

namespace GERV
{
    public class Attractor : MonoBehaviour
    {
        private const float MaxAttractionAngle = 180f;
    
        [Header("Attraction Settings")] 
        public Collider2D attractionCollider;
        public float attractionForce = 120f;
        public float dampeningForce = 200f;
        public float maxAttractionAngle = 45f; // Maximum angle (degrees) from the ship's forward direction

        [Header("Capture Settings")] 
        public Collider2D captureCollider;
        public float holdingForce = 2000f;
        public float holdingDampeningForce = 4000f;
        public uint maxCapturedObjects = 5;
        public float movementCompensationMod = 0.5f; // Modifier for position projection
    
        [Header("Release settings")]
        public float releaseForce = 5f;
        public float maxReleaseSpeed = 5f;

        [Header("Input Settings")]
        public KeyCode activationKey = KeyCode.Mouse0;
        public KeyCode releaseKey = KeyCode.Mouse1;

        private readonly HashSet<AttractableObject> _caughtObjects = new HashSet<AttractableObject>();
        private readonly HashSet<AttractableObject> _attractedObjects = new HashSet<AttractableObject>();

        private bool _setupValid = false;

        private Vector2 _previousPosition;

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

            if (Input.GetKey(releaseKey))
            {
                ReleaseObjects(this._caughtObjects);
            }
            else
            {
                HoldObjects(this._caughtObjects);
            
                if (Input.GetKey(activationKey))
                {
                    AttractObjects(this._attractedObjects);
                }
            }

            TrackPrevPosition();
        }

        private void TrackPrevPosition()
        {
            Vector2 currentPosition = transform.position;
            this._previousPosition = currentPosition;
        }

        private void HoldObjects(HashSet<AttractableObject> attractables)
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

        private void AttractObjects(HashSet<AttractableObject> attractables)
        {
            foreach (var attractable in attractables)
            {
                Rigidbody2D rb = attractable.Rb;

                if (rb == null) continue;

                Vector2 directionToTarget = (rb.position - (Vector2)transform.position).normalized;
                float angle = Vector2.Angle(transform.up, directionToTarget);

                if (angle > maxAttractionAngle) continue;
            
                // Attractables which have to be manipulated, may have to be treated differently.
                attractable.isManipulated = true;
            
                Vector2 attrVec = GetAttractionForce(rb, this.attractionForce, transform.position);
                Vector2 dampVec = GetDampeningForce(rb, attrVec.normalized, this.dampeningForce);

                rb.AddForce(attrVec + dampVec, ForceMode2D.Force);
            }
        }
    
        private void ReleaseObjects(HashSet<AttractableObject> attractables)
        {
            // Get the backward direction of the Attractor
            Vector2 backwardDirection = transform.up;

            foreach (var attractable in attractables)
            {
                Rigidbody2D rb = attractable.Rb;

                if (rb == null) continue;

                // Calculate the velocity component along the backward direction
                Vector2 velocityProjection = Vector2.Dot(rb.linearVelocity, backwardDirection) * backwardDirection;

                // Subtract the backward velocity component to neutralize it
                rb.linearVelocity = velocityProjection;
            
                if (rb.linearVelocity.magnitude > this.maxReleaseSpeed) continue;
            
                // Push objects away
                rb.AddForce(transform.up * this.releaseForce, ForceMode2D.Force);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            AttractableObject attractable = other.GetComponent<AttractableObject>();

            if(attractable == null || !attractable.IsValid) return;

            bool captureAreaisFull = this._caughtObjects.Count >= this.maxCapturedObjects;
        
            if (!captureAreaisFull && other.IsTouching(this.captureCollider))
            {
                this._caughtObjects.Add(attractable);
                attractable.SetStateCaptured();
                attractable.isManipulated = true;
            }
            if (other.IsTouching(this.attractionCollider))
            {
                this._attractedObjects.Add(attractable);
            }
        
            // Debug.Log($"Attracted: {this._attractedObjects.Count} Caught: {this._caughtObjects.Count}");

        } 

        private void OnTriggerExit2D(Collider2D other)
        {
            AttractableObject attractable = other.GetComponent<AttractableObject>();

            if(attractable == null || !attractable.IsValid) return;
        
            if (!other.IsTouching(this.captureCollider))
            {
                this._caughtObjects.Remove(attractable);
                attractable.UndoCapturedState();
            }
        
            if (!other.IsTouching(this.attractionCollider))
            {
                this._attractedObjects.Remove(attractable);
            }
        
            // Debug.Log($"Attracted: {this._attractedObjects.Count} Caught: {this._caughtObjects.Count}");
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
}
