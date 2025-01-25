using System;
using UnityEngine;

public class AttractableObject : MonoBehaviour, ICollectable
{
    public String captureLayerName = "CapturedObjects";
    public float damageAtCollection = 0;

    [Header("Random Rotate speed")]
    public float rotateSpeedSlow = -1;
    public float rotateSpeedFast = 1;
    private float rotateSpeed;


    [Header("Random Jitter Effect")]
    public bool randomJitter;
    public float jitterRange;
    public float jitterSpeed = 0.1f;
    private Vector3 nextJitterLocation;
    private Vector3 originalPosition = new(0, 0, 0);


    [Header("Orbit mother Ship")]
    public bool orbitMotherShip;
    public float orbitSpeedSlow;
    public float orbitSpeedFast;
    private float angleFromOrigin;
    private float actualSpeed;
    private float distanceFromOrigin;


    [Header("Attack mother ship")]
    public bool attackMotherShip;
    public float attackSpeedSlow;
    public float attackSpeedFast;
    public GameObject explosion;
    private float actualAttack;

    private Vector3 motherShipDirection;

    public bool isManipulated = false;

    private SpriteRenderer _spriteRenderer;
    private int _captureLayer;
    private int _originalLayer;
    private Vector3 motherShip = new(0, 0, 0);

    public bool IsValid { get; private set; }
    public bool IsCaptured { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    void Start()
    {
        Debug.Assert((randomJitter ? 1 : 0) + (attackMotherShip ? 1 : 0) + (orbitMotherShip ? 1 : 0) <= 1, "Exactly one boolean should be true.");

        originalPosition = transform.position;
        nextJitterLocation = transform.position;

        if (GameObject.Find("Mothership"))
        {
            motherShip = GameObject.Find("Mothership").transform.position;
        }

        rotateSpeed = UnityEngine.Random.Range(rotateSpeedSlow, rotateSpeedFast);
        actualSpeed = UnityEngine.Random.Range(orbitSpeedSlow, orbitSpeedFast);
        actualAttack = UnityEngine.Random.Range(attackSpeedSlow, attackSpeedFast);

        distanceFromOrigin = Vector2.Distance(transform.position, motherShip);
        Vector3 offsetOrigin = transform.position - motherShip;
        angleFromOrigin = Mathf.Atan2(offsetOrigin.y, offsetOrigin.x);
        motherShipDirection = motherShip - transform.position;

        this.Rb = GetComponent<Rigidbody2D>();
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this.IsValid = this.Rb != null && this._spriteRenderer != null;

        if (!this.IsValid) return;

        this._captureLayer = LayerMask.NameToLayer(this.captureLayerName);
        this._originalLayer = this.gameObject.layer;
    }

    public void SetStateCaptured()
    {
        if (this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = true;

        this.gameObject.layer = this._captureLayer;

        // Change the object's color to red
        this._spriteRenderer.color = Color.red;
    }

    public void UndoCapturedState()
    {
        if (!this.IsCaptured || !this.IsValid) return;
        this.IsCaptured = false;

        this.gameObject.layer = this._originalLayer;

        // Reset the object's color to white
        this._spriteRenderer.color = Color.white;
    }

    // This is when the object is collected by the mothership.
    // It's NOT the same as capturing/catching, which simply involves holding the object with the Mass Collector
    public void Collect()
    {
        Destroy(this.gameObject);
    }

    public string GetTag()
    {
        return this.gameObject.tag;
    }

    public IDamageEvent GetDamage()
    {
        return new RadioactiveDamageEvent(this.damageAtCollection);
    }

    void FixedUpdate()
    {
        if (!isManipulated)
        {
            if (randomJitter) JitterEffect();
            if (orbitMotherShip) OrbitMotherShip();
            if (attackMotherShip) AttackMotherShip();

            Quaternion quaternion = transform.rotation;
            quaternion.eulerAngles = new Vector3(0, 0, quaternion.eulerAngles.z + rotateSpeed);
            transform.rotation = quaternion;
        }
    }

    private void OrbitMotherShip()
    {
        angleFromOrigin += actualSpeed * 0.01f;

        float x = Mathf.Cos(angleFromOrigin) * distanceFromOrigin;
        float y = Mathf.Sin(angleFromOrigin) * distanceFromOrigin;

        Rb.MovePosition(new Vector3(x, y, 0) + motherShip);
    }

    private void AttackMotherShip()
    {
        Vector3 norm = motherShipDirection.normalized;
        Vector3 step = actualAttack * 0.01f * norm;

        Vector3 newPos = transform.position + step;
        Rb.MovePosition(newPos);
    }


    private void JitterEffect()
    {
        if (nextJitterLocation == transform.position)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            float distance = UnityEngine.Random.Range(0, jitterRange);

            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;

            nextJitterLocation = new Vector3(x, y, 0) + originalPosition;
        }

        Rb.MovePosition(Vector2.MoveTowards(transform.position, nextJitterLocation, Time.deltaTime * jitterSpeed));
    }

    public void OnDrawGizmos()
    {
        Vector3 loc = !Application.isPlaying ? transform.position : originalPosition;

        if (randomJitter && !isManipulated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(loc, jitterRange);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(nextJitterLocation, 0.1f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (attackMotherShip)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    public class RadioactiveDamageEvent : IDamageEvent
    {
        public RadioactiveDamageEvent(float damage)
        {
            this.IncomingDamage = damage;
        }

        public float IncomingDamage { get; }
        public Vector2? LocalAngle { get; }
        public Vector2? LocalPosition { get; }
    }
}