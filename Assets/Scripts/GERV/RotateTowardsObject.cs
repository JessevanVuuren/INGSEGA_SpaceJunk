using UnityEngine;

public class RotateTowardsObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Angular speed in degrees per second.")]
    public float rotationSpeed = 200f; // Degrees per second.


    [Header("Target Settings")]
    [Tooltip("If set, the object will track this Transform.")]
    public Transform targetTransform;
    
    [Tooltip("Track subject's mouse position instead of any transform.")]
    public bool trackMouse = false;

    [Tooltip(
        "Will only track objects beyond this distance. " +
        "Useful to prevent unpredictable spinning when mouse tracking is enabled and the mouse gets very close."
        )]
    public float minDistanceThreshold = 0.1f; // Distance threshold.
    
    private bool _trackingSettingsValid = false;

    void Start()
    {
        // if there's not mouse tracking and no transform, then there's nothing to track
        this._trackingSettingsValid = trackMouse || this.targetTransform != null;
    }
    
    void Update()
    {
        if (!this._trackingSettingsValid) return;
        
        // Decide which position to track: the mouse or a specified target.
        Vector3 targetPosition = GetTargetPosition();
        
        // Check if the target position is far enough to trigger rotation. Else return early
        if (Vector2.Distance(transform.position, targetPosition) < minDistanceThreshold) return;

        // Rotate towards the target position if beyond threshold distance.
        RotateTowardsTarget(targetPosition);
    }

    private Vector3 GetTargetPosition()
    {
        if (this.trackMouse)
        {
            // Default to the mouse position in world space.
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Set Z to 0 to match 2D space.
            return mousePosition;
        }

        // If a Transform is specified, use its position.
        return targetTransform.position;
    }

    void RotateTowardsTarget(Vector3 targetPosition)
    {
        // Calculate the direction vector from the ship to the target.
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Calculate the target angle in degrees.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Get the current angle of the ship.
        float currentAngle = transform.eulerAngles.z;

        // Smoothly rotate towards the target angle.
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // Apply the new rotation to the ship.
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
