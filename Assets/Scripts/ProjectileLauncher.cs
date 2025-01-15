using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    [Header("Missile Settings")]
    public GameObject missilePrefab; // Prefab of the missile to spawn
    public Transform launchPoint; // The position and direction where missiles are spawned
    public float launchForce = 10f; // Initial velocity of the missile

    [Header("Cooldown Settings")]
    public float cooldownDuration = 1f; // Time (in seconds) between consecutive launches
    public float bufferWindow = 0.25f; // Time (in seconds) after cooldown ends to accept buffered requests

    [Header("Input Settings")]
    public KeyCode fireKey = KeyCode.Space; // Key to fire the missile

    private float _lastLaunchTime = -Mathf.Infinity; // Tracks the last time a missile was launched
    private bool _bufferedRequest = false; // Tracks if there's a buffered launch request

    void Update()
    {
        // Check for the fire key press
        if (Input.GetKeyDown(fireKey))
        {
            HandleFireRequest();
        }

        // Check if a buffered request can be executed
        if (_bufferedRequest && Time.time >= _lastLaunchTime + cooldownDuration)
        {
            _bufferedRequest = false;
            LaunchMissile();
        }
    }

    private void HandleFireRequest()
    {
        float timeSinceLastLaunch = Time.time - _lastLaunchTime;

        // If the cooldown has passed, launch immediately
        if (timeSinceLastLaunch >= cooldownDuration)
        {
            LaunchMissile();
        }
        // Otherwise, buffer the request if within the buffer window
        else if (timeSinceLastLaunch >= cooldownDuration - bufferWindow)
        {
            _bufferedRequest = true;
        }
    }

    private void LaunchMissile()
    {
        // Ensure the missile prefab and launch point are set
        if (missilePrefab == null || launchPoint == null)
        {
            Debug.LogError("Missile prefab or launch point is not set!");
            return;
        }

        // Instantiate the missile at the launch point's position and orientation
        GameObject missile = Instantiate(missilePrefab, launchPoint.position, launchPoint.rotation);

        // Ensure the missile has a Rigidbody2D for physics
        Rigidbody2D missileRb = missile.GetComponent<Rigidbody2D>();
        if (missileRb != null)
        {
            // Apply an initial velocity in the forward direction of the launch point
            missileRb.linearVelocity = launchPoint.up * launchForce;
        }

        // Update the last launch time
        _lastLaunchTime = Time.time;

        // Clear any buffered requests
        _bufferedRequest = false;
    }
}
