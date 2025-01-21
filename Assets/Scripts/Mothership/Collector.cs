using UnityEngine;

public class Collector : MonoBehaviour
{
    public Collider2D CollectionCollider;

    [Tooltip("This is just a temporary value for testing purposes.")]
    public int scorePerCollectedDebris = 1;
    
    void Start()
    {
        bool isValid = !(this.CollectionCollider == null || !this.CollectionCollider.isTrigger);

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
        
        collectable.Collect();
        ScoreManager.Instance.IncrementScore(this.scorePerCollectedDebris);
    }
}
