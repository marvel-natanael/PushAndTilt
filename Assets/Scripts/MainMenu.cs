using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool isShown = false;
    public GameObject creditsObj;
    public GameObject settingsObj;

    private void Start()
    {
        creditsObj.gameObject.SetActive(false);
        settingsObj.gameObject.SetActive(false);
    }
    public void showSettings()
    {

    }
    public void setSFX()
    {

    }
    public void setMusic()
    {

    }
    public void showCredits()
    {
        isShown = !isShown;
        creditsObj.SetActive(isShown);
    }
}
