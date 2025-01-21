using UnityEngine;

public class Level_2_manager : MonoBehaviour
{
    public GameObject wave;
    public float waveSpeed;

    public GameObject waveDirection;


    private Vector3 waveDir;
    void Start()
    {
        waveDir = (waveDirection.transform.position - wave.transform.position).normalized;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wave.transform.position = wave.transform.position + Time.deltaTime * waveSpeed * waveDir;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        waveDir = (waveDirection.transform.position - wave.transform.position).normalized;
        Gizmos.DrawLine(wave.transform.position, wave.transform.position + waveDir * 100);

    }
}
