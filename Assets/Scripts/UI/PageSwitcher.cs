using System;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Activates and deactivates gameobjects. Meant for pages in a UI
 */
public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private int pageIndex = 0;
    public GameObject CurrentPage {get; private set;}

    void Start()
    {
        this.SwitchPage(this.pages[this.pageIndex]);
    }

    public void Next()
    {
        // pageIndex cannot get greater than the max page index
        // So you don't overflow, neither does it return to page with index 0
        pageIndex = Math.Min(pageIndex + 1, pages.Length - 1);
        this.SwitchPage(this.pages[pageIndex]);
    }
    
    public void Previous()
    {
        // prevent overflow below 0
        // neither does it return to page with max index
        pageIndex = Math.Max(pageIndex - 1, 0);
        this.SwitchPage(this.pages[pageIndex]);
    }

    public void SwitchPage(GameObject page)
    {
        if (this.CurrentPage != null)
        {
            this.CurrentPage.SetActive(false);
        }
        this.CurrentPage = page;
        this.CurrentPage.SetActive(true);
    }
}
