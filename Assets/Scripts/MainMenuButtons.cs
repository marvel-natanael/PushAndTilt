using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject[] panels;
    public Button sfxButton, musicButton;
    public Sprite musicOffSprite, musicOnSprite, sfxOnSprite, sfxOffSprite;
    private static bool soundOn = true, sfxOn = true;
    void Start()
    {
        foreach(GameObject obj in panels)
        {
            obj.gameObject.SetActive(false);
        }
    }
    public void showObject(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }
    public void hideObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
    public void vfxSwitch()
    {
        if (sfxOn)
        {
            sfxButton.GetComponent<Image>().sprite = sfxOffSprite;
        }
        else
        {
            sfxButton.GetComponent<Image>().sprite = sfxOnSprite;
        }
        sfxOn = !sfxOn;
    }
    public void switchSound()
    {
        if (soundOn)
        {
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        soundOn = !soundOn;
    }
}
