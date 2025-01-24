using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private int mainMenuNumber = 0;
    [SerializeField] private int levelSelectionNumber = 2;
    
    // Navigate to the level selection menu (Scene 2)
    public void GoToLevelSelection()
    {
        SceneManager.LoadScene(levelSelectionNumber);
    }

    // Retry the current level
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Navigate to the main menu (Scene 0)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuNumber);
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}