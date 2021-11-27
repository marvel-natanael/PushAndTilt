using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimManager : MonoBehaviour
{
    public static AnimManager Instance { get; private set; }

    public GameObject loseObj;
    public GameObject winObj;
    public GameObject text;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        text.SetActive(false);
        winObj.gameObject.SetActive(false);
        loseObj.gameObject.SetActive(false);
    }

    public void showDeathAnim()
    {
        loseObj.gameObject.SetActive(true);
        text.SetActive(true);
    }
    public void showWinAnim()
    {
        winObj.gameObject.SetActive(true);
        text.SetActive(true);
    }
}
