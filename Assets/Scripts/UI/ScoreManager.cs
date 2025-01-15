using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public TextMeshProUGUI scoreText;
    
    private int _score = 0;

    void Start()
    {
        ScoreManager.Instance = this;
        this.UpdateText();
    }

    private void UpdateText()
    {
        this.scoreText.text = this._score.ToString();
    }

    public void IncrementScore(int amount)
    {
        this._score += amount;
        this.UpdateText();
    }
}
