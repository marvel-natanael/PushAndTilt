using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject creditsPanel;
    void Start()
    {
        creditsPanel.gameObject.SetActive(false);
    }
    public void showObject(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }
    public void hideObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
}
