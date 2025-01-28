using System;
using UnityEngine;

namespace UI
{
    public class HudRadar : MonoBehaviour
    {
        public Vector2 centerOffset = new Vector2(6f, 4f); 
        public RadarIndicator[] radarIndicators;

        // Update is called once per frame
        void Update()
        {
            TrackableObjectCollectionManager collectionManager = TrackableObjectCollectionManager.Instance;
        
            foreach (RadarIndicator indicator in radarIndicators)
            {
                TrackableObject target = collectionManager.GetNearest(indicator.objTag);

                if (target == null) continue;
            
                Vector2 deltaFromRadar = target.transform.position - this.transform.position;
            
                float distanceFromRadar = deltaFromRadar.magnitude;
            
                if (distanceFromRadar < indicator.minDistance)
                {
                    indicator.hudElement.SetActive(false);
                    continue;
                }
                else
                {
                    indicator.hudElement.SetActive(true);
                }
            
                Vector2 direction = deltaFromRadar.normalized;

                float excessOffset = Math.Clamp(distanceFromRadar / this.centerOffset.magnitude,0f,1f);
            
                Vector2 correctedOffset = this.centerOffset * excessOffset;
            
                Vector2 newPos = (Vector2)this.transform.position + direction * correctedOffset;
                indicator.hudElement.transform.position = newPos;
            
                Vector2 arrowDirection = ((Vector2)target.transform.position - newPos).normalized;
            
                float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
                indicator.arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            
            }
        }
    
        [System.Serializable]
        public class RadarIndicator
        {
            public GameObject hudElement;
            public GameObject arrow;
            public float minDistance = 6f;
            public String objTag = "Garbage";
        }
    }
}
