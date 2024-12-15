using UnityEditor.UIElements;
using UnityEngine;

public class WormHole : MonoBehaviour
{
    public float rotateSpeed = 1;

    public float duration;
    public float maxSize = 1;
    public float growShrinkDuration = 5;

    public ParticleSystem stars;
    public ParticleSystem dust;
    public GameObject disk;
    private float currentTime;



    void Start()
    {
        currentTime = Time.time;
        SetSize(0);
    }


    void FixedUpdate()
    {
        if (Time.time < currentTime + growShrinkDuration) {
            SetSize(EaseInOutQuint((Time.time - currentTime) / growShrinkDuration) * maxSize);
        }
        
        if (Time.time > currentTime + growShrinkDuration + duration) {
            SetSize(1 - EaseInOutQuint((Time.time - currentTime - duration - growShrinkDuration) / growShrinkDuration) * maxSize);
        }

        if (Time.time > currentTime + (growShrinkDuration * 2) + duration) {
            Destroy(gameObject);
        }

        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(0, 0, q.eulerAngles.z + rotateSpeed);
        transform.rotation = q;
    }

    private void SetSize(float size) {
        disk.transform.localScale = new Vector3(size, size, 1);
        stars.transform.localScale = new Vector3(size, size, 1);
        dust.transform.localScale = new Vector3(size, size, 1);
    }

    private float EaseInOutQuint(float x)
    {
        return x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
    }
}
