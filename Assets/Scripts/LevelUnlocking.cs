using UnityEngine;
using UnityEngine.UI;

public class LevelUnlocking : MonoBehaviour
{

    public Button level1;
    public bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.level1.enabled = isActive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
