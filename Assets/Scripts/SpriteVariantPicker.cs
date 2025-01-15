using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteVariantPicker : MonoBehaviour
{
    public Sprite[] sprites;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(sprites.Length == 0) return;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        spriteRenderer.sprite = this.sprites[Random.Range(0, sprites.Length - 1)];
    }
}
