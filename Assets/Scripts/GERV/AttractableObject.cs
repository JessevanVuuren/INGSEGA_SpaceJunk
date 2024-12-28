using UnityEngine;


public class AttractableObject : MonoBehaviour
{
    public Rigidbody2D _rb { get; private set; }
    void Start()
    {
        this._rb = GetComponent<Rigidbody2D>();
        
        // if (this._rb == null) return;
    }
}
