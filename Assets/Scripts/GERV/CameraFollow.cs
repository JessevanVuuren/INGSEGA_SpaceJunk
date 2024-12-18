using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("GameObject to follow.")]
    public Transform subject; // Assign the subject's Transform in the Inspector

    // Flags to control which axes of position are updated
    public bool updatePositionX = true;
    public bool updatePositionY = true;
    public bool updatePositionZ = true;

    // Flags to control whether to update rotation
    public bool matchSubjectRotation = false; // If true, match the subject's rotation
    
    [Tooltip("If true, the script will remember the position that the camera had relative to the subject at " +
             "the time when the game started and use it as an offset.")]
    public bool useInitialDistanceAsOffset = true;
    
    private Vector3 _offset = new Vector3(0f,0f,0f);

    private void Start()
    {
        if (!this.useInitialDistanceAsOffset) return;
        
        this._offset = this.transform.position - this.subject.position;
    }

    void LateUpdate()
    {
        if (subject == null) return;
        
        // Calculate target position
        Vector3 targetPosition = subject.position + _offset;
        Vector3 currentPosition = transform.position;

        // Conditionally update each axis of the position
        float newX = updatePositionX ? targetPosition.x : currentPosition.x;
        float newY = updatePositionY ? targetPosition.y : currentPosition.y;
        float newZ = updatePositionZ ? targetPosition.z : currentPosition.z;

        transform.position = new Vector3(newX, newY, newZ);

        // Conditionally update rotation
        if (matchSubjectRotation)
        {
            // Match the subject's rotation exactly
            transform.rotation = subject.rotation;
        }
    }
}
