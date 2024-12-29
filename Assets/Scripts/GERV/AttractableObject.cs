using UnityEngine;


public class AttractableObject : MonoBehaviour
{
    public Rigidbody2D _rb { get; private set; }
    private Transform _originalParentTransform;
    void Start()
    {
        this._rb = GetComponent<Rigidbody2D>();
        
        // if (this._rb == null) return;
    }

    public void SetParent(Transform p)
    {
        if (this._originalParentTransform == null)
        {
            this._originalParentTransform = this.transform.parent;
        }
        this.transform.SetParent(p);
    }

    public void ResetParent()
    {
        this.transform.SetParent(this._originalParentTransform);
    }
}
