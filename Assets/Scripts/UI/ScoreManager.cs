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
    [FormerlySerializedAs("levelInt")] [SerializeField] private int nextLevelToUnlock;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private Objective[] objectives = Array.Empty<Objective>();

    void Start()
    {
        ScoreManager.Instance = this;
        
        UpdateText();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            objectives[0].amount++;
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            objectives[0].amount--;
        }

        UpdateText();
        CheckLevelEndCondition();
    }

    private void UpdateText()
    {
        foreach (var objective in this.objectives)
        {
            String newScore = $"{objective.amount.ToString()}/{objective.amountRequired.ToString()}";
            objective.scoreText.text = newScore;
        }
    }

    public void IncrementScore(String collectableTag, int amount)
    {
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
        PlayerPrefs.SetInt("LevelsFinished", nextLevelToUnlock);
        // SceneManager.LoadScene(2);
        
        if (this.winMenu == null) return;
        
        this.winMenu.SetActive(true);
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
