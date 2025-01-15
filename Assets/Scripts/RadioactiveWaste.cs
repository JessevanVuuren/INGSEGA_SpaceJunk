using UnityEngine;
using System.Linq;
using UnityEditor;

public class RadioactiveWaste : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public Texture2D texture;
    private Sprite[] sprites;
    public float rotateSpeedSlow = 0;
    public float rotateSpeedFast = 1;
    private SpriteRenderer spriteRenderer;
    private float rotateSpeed;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        string spriteSheet = AssetDatabase.GetAssetPath(texture);
        sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length - 1)];
        rotateSpeed = Random.Range(rotateSpeedSlow, rotateSpeedFast);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(0,0, q.eulerAngles.z + rotateSpeed);
        transform.rotation = q;
        
    }


}
