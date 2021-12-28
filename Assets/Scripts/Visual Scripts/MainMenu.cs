using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    private bool isShown = false;
    private bool isMusicOn, isSfxOn;
    private bool isServerBrowser = false;
    public bool directConnectActive;
    public Button musicButton, sfxButton;
    public Sprite musicOn, musicOff, sfxOn, sfxOff;
    public GameObject creditsObj;
    public GameObject settingsObj;
    public AudioMixer sfxMixer, musicMixer;

    public RectTransform mainMenu, serverMenu;

    private void Start()
    {
        mainMenu.DOAnchorPos(Vector2.zero, 0.25f);
        creditsObj.gameObject.SetActive(false);
        settingsObj.gameObject.SetActive(false);
    }

    //Transition
    public void swipeRight()
    {
        FindObjectOfType<ServerBrowserScript>().ClearBrowserList();
        UI_CloseAllPopUps();
        mainMenu.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        serverMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        isServerBrowser = true;
    }

    public void swipeLeft()
    {
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        serverMenu.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        isServerBrowser = false;
    }

    //Settings
    public void showSettings()
    {
        isShown = !isShown;
        settingsObj.gameObject.SetActive(isShown);
        creditsObj.gameObject.SetActive(false);
    }

    public void setSFX()
    {
        isSfxOn = !isSfxOn;
        switch (isSfxOn)
        {
            case true:
                {
                    sfxMixer.SetFloat("sfxVolume", -80);
                    sfxButton.GetComponent<Image>().sprite = sfxOff;
                    break;
                }
            case false:
                {
                    sfxMixer.SetFloat("sfxVolume", 0);
                    sfxButton.GetComponent<Image>().sprite = sfxOn;
                    break;
                }
        }
    }

    public void UI_QuitApplication()
    {
        Application.Quit();
    }

    private void UI_CloseAllPopUps()
    {
        creditsObj.gameObject.SetActive(false);
        settingsObj.gameObject.SetActive(false);
    }

    public void showCredits()
    {
        isShown = !isShown;
        creditsObj.gameObject.SetActive(isShown);
        settingsObj.gameObject.SetActive(false);
    }

    public void setMusic()
    {
        isMusicOn = !isMusicOn;
        switch (isMusicOn)
        {
            case true:
                {
                    musicMixer.SetFloat("musicVolume", -80);
                    musicButton.GetComponent<Image>().sprite = musicOff;
                    break;
                }
            case false:
                {
                    musicMixer.SetFloat("musicVolume", 0);
                    musicButton.GetComponent<Image>().sprite = musicOn;
                    break;
                }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isServerBrowser)
            {
                if (directConnectActive)
                {
                    FindObjectOfType<ServerBrowserScript>().UI_HideDirectConnect();
                    directConnectActive = false;
                }
                else
                {
                    swipeLeft();
                }
            }
            else
            {
                UI_CloseAllPopUps();
            }
        }
    }
}