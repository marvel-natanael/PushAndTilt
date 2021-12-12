using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Mirror;

public class LobbyUIScript : MonoBehaviour
{
    private GameManager gameManager;
    private MyNetworkManager netManager;
    [SerializeField] private LobbyManager manager;
    [SerializeField] private TextMeshProUGUI hostNameLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;
    [SerializeField] private Button toggleReadyButton;

    private void Awake()
    {
        if (!netManager)
        {
            netManager = FindObjectOfType<MyNetworkManager>();
        }
        if (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (!manager)
        {
            manager = FindObjectOfType<LobbyManager>();
        }
        if (!hostNameLabel)
        {
            hostNameLabel = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        if (!statusLabel)
        {
            hostNameLabel = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        hostNameLabel.text = "Host Name: " + netManager.HostName;
        statusLabel.text = "Waiting for players...";
    }

    private void Update()
    {
        if (gameManager.PlayersConnected < 2)
        {
            toggleReadyButton.interactable = false;
        }
        else
        {
            toggleReadyButton.interactable = true;
        }
    }
}