using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Main()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void ToLevel(int load_level)
    {
        SceneManager.LoadSceneAsync(load_level);
    }
}
