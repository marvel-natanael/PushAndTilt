using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Mirror;

public class LobbyUIScript : MonoBehaviour
{
    private MyNetworkManager netManager;
    private LobbyManager lobbbyManager;

    [SerializeField] private GameObject playerJoinPrefab;
    [SerializeField] private GameObject errorLabelPrefab;
    [SerializeField] private TextMeshProUGUI hostNameLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;
    [SerializeField] private TextMeshProUGUI countDownNumberLabel;

    public void Start()
    {
        if (!(netManager = FindObjectOfType<MyNetworkManager>()))
        {
            Debug.LogError($"{ToString()}: netManager not found");
        }
        hostNameLabel.text = "Host Name: " + netManager.HostName;
        statusLabel.text = "Waiting for players...";
        CW_numEmpty();
    }

    public void UI_ShowJoined(string name)
    {
        var temp = Instantiate(playerJoinPrefab, gameObject.transform);
        temp.GetComponent<TextMeshProUGUI>().text = $"{name} has joined the game!";
    }

    public void UI_ShowError(string msg)
    {
        var label = Instantiate(errorLabelPrefab, gameObject.transform);
        label.GetComponent<TextMeshProUGUI>().text = msg;
    }

    public void Button_SetReady()
    {
        if (lobbbyManager = FindObjectOfType<LobbyManager>())
            lobbbyManager.CmdSetPlayerReadyState(!lobbbyManager.LocalReady);
        else
            Debug.LogError($"{ToString()}: lobbyManager not found");
    }

    public void CW_numUpdate(float time)
    {
        countDownNumberLabel.text = time.ToString("0.0");
    }

    public void CW_numEmpty()
    {
        countDownNumberLabel.text = string.Empty;
    }
}