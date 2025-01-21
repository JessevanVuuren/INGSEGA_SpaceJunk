using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI collectAmount;
    public int amountNeedToCollect;
    public int levelInt;

    private int _score = 0;
    private int totalAmount;

    void Start()
    {
        ScoreManager.Instance = this;
        totalAmount = GameObject.FindGameObjectsWithTag("Garbage").Length;
        
        UpdateText();
    }

    private void UpdateText()
    {
        scoreText.text = _score.ToString();
        totalText.text = totalAmount.ToString();
        collectAmount.text = amountNeedToCollect.ToString();
    }

    public void IncrementScore(int amount)
    {
        _score += amount;
        totalAmount -= amount;

        UpdateText();
        EndLevel();
    }

    public void EndLevel() {
        if (_score >= amountNeedToCollect) {
            PlayerPrefs.SetInt("LevelsFinished", levelInt);
            SceneManager.LoadScene(2);
        }
    }
}
