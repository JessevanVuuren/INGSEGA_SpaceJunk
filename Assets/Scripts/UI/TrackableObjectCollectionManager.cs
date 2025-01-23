using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TrackableObjectCollectionManager : MonoBehaviour
{
    public static TrackableObjectCollectionManager Instance { get; private set; }
    
    [SerializeField] private Transform tracker; // The reference object
    public float updateInterval = 0.25f; // Interval in seconds
    [Tooltip("Set this to a value greater than 0 if you think objects that are too close aren't worth considering.\n" +
             "Useful for tracking on the HudRadar.")]
    public float minDistanceForNearestObject = 0f;
    
    public Dictionary<string, HashSet<TrackableObject>> _trackableCollections { get; private set; } = new Dictionary<string, HashSet<TrackableObject>>();
    private Dictionary<string, TrackableObject> _nearestTrackableCache = new Dictionary<string, TrackableObject>();
    
    private Coroutine _updateCoroutine;

    private bool _updateClosestObjects = false;

    void Start()
    {
        Instance = this;
        
        _updateClosestObjects = tracker != null;

        if (_updateClosestObjects)
        {
            // Start the periodic proximity check
            _updateCoroutine = StartCoroutine(UpdateNearestTrackables());
        }
    }

    public void Register(string collection, [NotNull] TrackableObject trackableObject)
    {
        if (trackableObject == null)
        {
            throw new ArgumentNullException(nameof(trackableObject));
        }
        
        if (!_trackableCollections.ContainsKey(collection))
        {
            _trackableCollections[collection] = new HashSet<TrackableObject>();
        }
        
        _trackableCollections[collection].Add(trackableObject);
    }

    public void Unregister(string collection, [NotNull] TrackableObject trackableObject)
    {
        if (trackableObject == null)
        {
            throw new ArgumentNullException(nameof(trackableObject));
        }
        
        if (!_trackableCollections.ContainsKey(collection)) return;
        
        _trackableCollections[collection].Remove(trackableObject);
    }
    
    public TrackableObject GetNearest(string collection)
    {
        return _nearestTrackableCache.GetValueOrDefault(collection);
    }

    private IEnumerator UpdateNearestTrackables()
    {
        while (true)
        {
            // Iterate over each collection
            foreach (var collection in _trackableCollections)
            {
                string collectionName = collection.Key;
                HashSet<TrackableObject> trackables = collection.Value;

                // Find the nearest trackableObject in this collection
                TrackableObject nearestTrackableObject = null;
                float shortestDistance = float.MaxValue;

                foreach (TrackableObject trackable in trackables)
                {
                    // if (trackable == null) continue; // Skip null objects
                    
                    float distance = Vector2.Distance(tracker.position, trackable.transform.position);

                    if (distance >= shortestDistance || distance < minDistanceForNearestObject) continue;
                    
                    shortestDistance = distance;
                    nearestTrackableObject = trackable;
                }

                // if(nearestTrackableObject == null) continue;
                
                // Update the cache
                _nearestTrackableCache[collectionName] = nearestTrackableObject;
            }
            
            // Debug.Log($"{_nearestTrackableCache.Count} {this._nearestTrackableCache}");

            // Wait for the next update interval
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void OnDisable()
    {
        if (_updateCoroutine == null) return;
        
        StopCoroutine(_updateCoroutine);
        _updateCoroutine = null;
    }
}
