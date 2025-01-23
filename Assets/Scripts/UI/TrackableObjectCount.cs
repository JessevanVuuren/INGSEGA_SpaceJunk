using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackableObjectCount : MonoBehaviour
{
    [SerializeField] private TrackableObjectCollectionManager trackableObjectCollectionManager;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private List<String> tagWhiteList;

    // Update is called once per frame
    void Update()
    {
        this.UpdateCount();
    }

    private void UpdateCount()
    {
        Dictionary<string, HashSet<TrackableObject>> objects = trackableObjectCollectionManager._trackableCollections;
        
        String text = "";

        foreach (KeyValuePair<string, HashSet<TrackableObject>> pair in objects)
        {
            if (!tagWhiteList.Contains(pair.Key)) continue;
            
            text += $"{pair.Key}: {pair.Value.Count}\n";
        }
        
        this.countText.text = text;
    }
}
