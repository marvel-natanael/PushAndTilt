using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    public static AnimManager Instance { get; private set; }

    public GameObject loseObj;
    public GameObject winObj;
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

    public void showDeathAnim()
    {

    }
    public void showWinAnim()
    {

    }
}
