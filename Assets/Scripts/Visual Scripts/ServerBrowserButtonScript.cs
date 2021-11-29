using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerBrowserButtonScript : MonoBehaviour
{
    private TextMeshProUGUI hostName;
    private TextMeshProUGUI address;
    private TextMeshProUGUI playerCount;
    private int s_playerCount;

    public string HostName => hostName.text;
    public string Address => address.text;
    public int PlayerCount => s_playerCount;

    public void Awake()
    {
        if (!hostName)
        {
            hostName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        if (!address)
        {
            address = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        if (!playerCount)
        {
            playerCount = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Sets button instance's texts to display in Server Browser
    /// </summary>
    /// <param name="_hostName">Server name</param>
    /// <param name="_address">Server socket info (ip:port)</param>
    /// <param name="_playerCount">Server current playerCount</param>
    /// <returns><c>true</c>, if successful</returns>
    public bool SetButtonInfo(string _hostName, string _address, int _playerCount)
    {
        hostName.text = _hostName;
        address.text = _address;
        s_playerCount = _playerCount;
        playerCount.text = _playerCount.ToString();
        return true;
    }

    public void Selected()
    {
        FindObjectOfType<ServerBrowserScript>().CurrentSelected = this;
    }

    public void UpdatePlayerCount(int count)
    {
        s_playerCount = count;
    }
}