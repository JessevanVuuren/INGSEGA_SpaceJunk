using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Activates and deactivates gameobjects. Meant for pages in a UI
 */
public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private int pageIndex = 0;
    
    [Tooltip("Which page # out of the total # of pages is the player on? e.g. '1/2' or '2/10'.")]
    [SerializeField] private TextMeshProUGUI pageCounter;
    public GameObject CurrentPage {get; private set;}

    void Start()
    {
        this.SwitchPage(this.pages[this.pageIndex]);
    }

    public void Next()
    {
        // pageIndex cannot get greater than the max page index
        // So you don't overflow, neither does it return to page with index 0
        int newIndex = Math.Min(pageIndex + 1, pages.Length - 1);
        this.SetPageIndex(newIndex);
    }
    
    public void Previous()
    {
        // prevent overflow below 0
        // neither does it return to page with max index
        int newIndex = Math.Max(pageIndex - 1, 0);
        this.SetPageIndex(newIndex);
    }
    
    public void SetPageIndex(int index)
    {
        this.pageIndex = index;
        String pageCounterStr = $"{index + 1}/{pages.Length}";
        this.pageCounter.text = pageCounterStr;
        this.SwitchPage(this.pages[index]);
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
