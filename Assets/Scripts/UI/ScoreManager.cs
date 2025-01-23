using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    // public TextMeshProUGUI scoreText;
    // public TextMeshProUGUI totalText;
    // public TextMeshProUGUI collectAmount;
    // public int amountNeedToCollect;
    public int levelInt;

    [SerializeField] public Objective[] objectives = Array.Empty<Objective>();
 
    // private int _score = 0;
    // private int totalAmount;

    void Start()
    {
        ScoreManager.Instance = this;
        // totalAmount = GameObject.FindGameObjectsWithTag("Garbage").Length;
        
        UpdateText();
    }

    private void UpdateText()
    {
        // scoreText.text = _score.ToString();
        // totalText.text = totalAmount.ToString();
        // collectAmount.text = amountNeedToCollect.ToString();

        foreach (var objective in this.objectives)
        {
            String newScore = $"{objective.amount.ToString()}/{objective.amountRequired.ToString()}";
            objective.scoreText.text = newScore;
        }
    }

    public void IncrementScore(String collectableTag, int amount)
    {
        // _score += amount;
        // totalAmount -= amount;
        
        IEnumerable<Objective> objectives = this.objectives.Where(objective => objective.objectTag == collectableTag);
        
        if (objectives.Count() == 0) return;
        
        Objective objective = objectives.First();
        objective.amount += amount;
        
        UpdateText();
        CheckLevelEndCondition();
    }

    public void CheckLevelEndCondition() {
        foreach (var objective in this.objectives)
        {
            // If even one objective isn't met, return
            if (objective.amount < objective.amountRequired) return;
        }
        
        // If all objectives are met, go to next level
        PlayerPrefs.SetInt("LevelsFinished", levelInt);
        SceneManager.LoadScene(2);
    }

    [System.Serializable]
    public class Objective
    {
        public String objectTag;
        public int amount;
        public int amountRequired;
        public TextMeshProUGUI scoreText;
    }
}
