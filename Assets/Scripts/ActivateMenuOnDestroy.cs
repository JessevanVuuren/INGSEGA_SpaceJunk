using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ActivateMenuOnDestroy : MonoBehaviour
{
    [Tooltip("The menu to activate when this GameObject is destroyed.")]
    [SerializeField] private GameObject menu;
    public bool showSceneInsteadOfMenu = false;
    public int sceneToShow = 0;

    private void OnDestroy()
    {
        // Early return to prevent the script from needlessly throwing errors when the scene is being cleaned up.
        if(!Application.isPlaying) return;

        if (showSceneInsteadOfMenu)
        {
            SceneManager.LoadScene(this.sceneToShow);
            return;
        }
        
        // Check if  menu is assigned
        if (menu == null)
        {
            Debug.LogError($"{nameof(ActivateMenuOnDestroy)}: menu is not assigned in the inspector.");
            return;
        }
        
        menu.SetActive(true);
    }
}