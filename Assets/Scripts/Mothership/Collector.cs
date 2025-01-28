using System;
using UI;
using UnityEngine;

namespace Mothership
{
    public class Collector : MonoBehaviour
    {
        public Collider2D CollectionCollider;
        public HealthController HealthController;

        [Tooltip("This is just a temporary value for testing purposes.")]
        public int scorePerCollectedDebris = 1;
    
        void Start()
        {
            bool isValid = !(this.CollectionCollider == null || !this.CollectionCollider.isTrigger || this.HealthController == null);

            if (isValid) return;

            this.enabled = false;
        
            Debug.LogError($"Collector {nameof(this.gameObject)} isn't working! Invalid settings.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ICollectable collectable = other.GetComponent<ICollectable>();

            if (collectable == null) return;

            this.Collect(collectable);
        }

        private void Collect(ICollectable collectable)
        {
            // Debug.Log($"Collected! {nameof(collectable)}");
        
            String collectableTag = collectable.GetTag();
            IDamageEvent damage = collectable.GetDamage();
            if (damage.IncomingDamage > 0f)
            {
                this.HealthController.Damage(damage);
            }
            collectable.Collect();
            ScoreManager.Instance.IncrementScore(collectableTag, this.scorePerCollectedDebris);
        }
    }
}
