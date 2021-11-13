using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static bool isShown = false;
    public Button creditsObj;
    public Button settingsObj;

    private void Start()
    {
        creditsObj.gameObject.SetActive(false);
        settingsObj.gameObject.SetActive(false);
    }

    public void showSettings()
    {
        isShown = !isShown;
        settingsObj.gameObject.SetActive(isShown);
        creditsObj.gameObject.SetActive(false);
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
        creditsObj.gameObject.SetActive(isShown);
        settingsObj.gameObject.SetActive(false);
    }
}