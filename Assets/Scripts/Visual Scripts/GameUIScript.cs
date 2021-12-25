using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIScript : MonoBehaviour
{
    private MyNetworkManager netManager;

    private void Awake()
    {
        if (netManager = FindObjectOfType<MyNetworkManager>())
        {
            Debug.LogError($"{ToString()}: netManager not found");
        }
    }
}