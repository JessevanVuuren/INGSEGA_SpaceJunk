    using System;
    using UnityEngine;

    public class TrackableObject : MonoBehaviour
    {
        [Tooltip("This can be left as null, but can also be specified explicitly if needed.")]
        [SerializeField] private TrackableObjectCollectionManager trackableObjectCollectionManager;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            TrackableObjectCollectionManager collectionManager = this.GetCollectionManager();

            if (collectionManager == null)
            {
                Debug.LogError("TrackableObjectCollectionManager is null");

                return;
            };
            
            String trackableTag = this.gameObject.tag;
            
            if (trackableTag == null || trackableTag.Equals(""))
            {
                Debug.LogError("TrackableObject's Tag is empty!");
                
                return;
            };
            
            collectionManager.Register(trackableTag, this);
        }

        void OnDestroy()
        {
            TrackableObjectCollectionManager collectionManager = this.GetCollectionManager();
            
            collectionManager.Unregister(this.gameObject.tag, this);
        }
        
        private TrackableObjectCollectionManager GetCollectionManager()
        {
            return 
                this.trackableObjectCollectionManager != null 
                    ? this.trackableObjectCollectionManager 
                    : TrackableObjectCollectionManager.Instance;
        }
    }
