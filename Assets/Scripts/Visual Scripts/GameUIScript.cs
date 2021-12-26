using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIScript : MonoBehaviour
{
    private MyNetworkManager netManager;
    [SerializeField] private GameObject confUI;

    public void GameUI_PauseButtonFunc()
    {
        if (!confUI.activeSelf)
            confUI.SetActive(true);
    }

    public void GameUI_DisconnectConfirmButtonFunc()
    {
        netManager.Disconnect();
    }

    private void Start()
    {
        if (!(netManager = FindObjectOfType<MyNetworkManager>()))
        {
            Debug.LogError($"{ToString()}: netManager not found");
        }
        confUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confUI.activeSelf)
            {
                confUI.SetActive(false);
            }
        }
    }
}