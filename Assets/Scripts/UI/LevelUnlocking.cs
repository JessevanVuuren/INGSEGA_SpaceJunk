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
        int nextLevel = PlayerPrefs.GetInt("LevelsFinished", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];
            button.interactable = i <= nextLevel;
        }
    }
}