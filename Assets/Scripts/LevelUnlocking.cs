using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlocking : MonoBehaviour
{
    public Button[] levelButtons; // Assign your buttons in the Inspector
    public int unlockedLevels = 0; // Number of levels unlocked
 
    void Start()
    {
        UpdateLevelButtons();
    }
 
    private void UpdateLevelButtons()
    {
        for (int i = 0; i < this.levelButtons.Length; i++)
        {
            String keyName = $"LevelsFinished";
            
            int nextLevel = PlayerPrefs.GetInt(keyName, 0);
            
            bool isUnlocked = i <= nextLevel;
            
            Button button = levelButtons[i];
            
            button.interactable = isUnlocked;
            
            // // Optional: Update visual state (color or sprite)
            // var buttonText = button.GetComponentInChildren<Text>();
            // if (buttonText != null)
            // {
            //     buttonText.color = isUnlocked ? Color.white : Color.gray;
            // }
        }
    }
}