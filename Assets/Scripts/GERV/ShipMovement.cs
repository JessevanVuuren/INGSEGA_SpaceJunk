using UnityEngine;

namespace GERV
{
    public class ShipMovement : MonoBehaviour
    {
        [Tooltip("The acceleration force applied to the ship.")]
        public float accelerationForce = 10f;

        [Tooltip("Modifier applied to the dampening effect.")]
        public float extraDampeningFactor = 1.5f;
    
        [Tooltip("Ship velocity is set to zero once it's magnitude reaches below this value. " +
                 "Prevents jittery 'ghost movement' standing still.")]
        public float haltVelocity = 0.15f;
    
        [Tooltip("Ship does not accelerate above this speed, " +
                 "but could still reach higher velocities when pushed by something.")]
        public float maxSpeed = float.MaxValue;

        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            // Calculate the desired movement direction based on input
            Vector2 inputDirection = GetInputDirection();
        
            // DOn't apply forces and set velocity to zero when no acceleration is needed and velocity is near-zero 
            if (inputDirection == Vector2.zero && this.rb.linearVelocity.magnitude < this.haltVelocity)
            {
                this.rb.linearVelocity = Vector2.zero; // Set velocity to zero when near halt threshold
                return;
            }

            Vector2 dampeningDirection = GetDampeningDirection(inputDirection);

            Vector2 forceDirection = (dampeningDirection + inputDirection).normalized;
        
            // Apply increase (or decrease) dampening force if needed
            forceDirection += dampeningDirection * (extraDampeningFactor - 1f);

            if (this.rb.linearVelocity.magnitude > this.maxSpeed)
            {
                forceDirection -= this.rb.linearVelocity.normalized;
            }
        
            // Apply force in the desired direction
            rb.AddForce(forceDirection * accelerationForce, ForceMode2D.Force);
        }

        /// <summary>
        /// Calculates the normalized movement direction based on WASD input.
        /// For removing unwanted velocity components
        /// </summary>
        /// <returns>Normalized direction vector.</returns>
        private Vector2 GetInputDirection()
        {
            // Check for input on movement keys
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
        
            // Combine inputs into a single direction vector
            Vector2 inputDirection = new Vector2(horizontal, vertical);

            // Normalize the vector to prevent diagonal movement from being faster
            return inputDirection.normalized;
        }

        /// <summary>
        /// Applies a dampening force to cancel velocity components not aligned with the desired direction.
        /// </summary>
        /// <param name="inputDirection">The desired movement direction.</param>
        private Vector2 GetDampeningDirection(Vector2 inputDirection)
        {
            // Project current velocity onto the input direction
            Vector2 currentDirection = rb.linearVelocity.normalized;
        
            // Calculate the velocity that needs to be canceled
            return inputDirection - currentDirection;
        }
    }
}
