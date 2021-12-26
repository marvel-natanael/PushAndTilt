using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ServerBrowserScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonTemplate;
    private Dictionary<string, GameObject> registered;

    [HideInInspector] public ServerBrowserButtonScript CurrentSelected;

    private void Awake()
    {
        registered = new Dictionary<string, GameObject>();
        ClearBrowserList();
        FindObjectOfType<MyNetworkDiscovery>().StartDiscovery();
    }

    /// <summary>
    /// Button function that starts a client
    /// </summary>
    public void StartClient()
    {
        var clientField = GameObject.FindGameObjectWithTag("clientNameField").GetComponent<TMP_InputField>();
        if (clientField.text.Length > 1)
        {
            if (CurrentSelected)
            {
                var netManager = FindObjectOfType<MyNetworkManager>();
                netManager.networkAddress = CurrentSelected.Address;
                netManager.HostName = CurrentSelected.HostName;
                netManager.LocalPlayerName = clientField.text;

                FindObjectOfType<MyNetworkManager>().StartClient();
            }
            else
            {
                Debug.Log("ServerBrowserScript.cs/StartClient(): nothing is currently selected!");
            }
        }
        else
        {
            Debug.Log("ServerBrowserScript.cs/StartClient(): client name is not set!");
            clientField.transform.parent.gameObject.GetComponent<Animator>().Play("Flash");
        }
    }

    /// <summary>
    /// Button function that starts a host
    /// </summary>
    public void StartHost()
    {
        var clientField = GameObject.FindGameObjectWithTag("clientNameField").GetComponent<TMP_InputField>();
        var serverField = GameObject.FindGameObjectWithTag("hostNameField").GetComponent<TMP_InputField>();
        if (clientField.text.Length > 0)
        {
            if (serverField.text.Length > 0)
            {
                if (!NetworkServer.active && !NetworkClient.isConnected)
                {
                    var netManager = GameObject.FindGameObjectWithTag("networkManager").GetComponent<MyNetworkManager>();
                    if (netManager)
                    {
                        netManager.gameObject.GetComponent<MyNetworkDiscovery>().StopDiscovery();
                        netManager.HostName = GameObject.FindGameObjectWithTag("hostNameField").GetComponent<TMP_InputField>().text;
                        netManager.LocalPlayerName = clientField.text;
                        netManager.StartHost();
                    }
                    else
                    {
                        Debug.LogError("ServerBrowserScript.cs/StartHost(): network manager is not found!");
                    }
                }
            }
            else
                serverField.transform.parent.gameObject.GetComponent<Animator>().Play("Flash");
        }
        else
        {
            Debug.Log("ServerBrowserScript.cs/StartHost(): client name is not set!");
            clientField.transform.parent.gameObject.GetComponent<Animator>().Play("Flash");
        }
    }

    /// <summary>
    /// Sets a new server entry in the server browser
    /// </summary>
    /// <param name="response">response data which contains hostname and player count</param>
    /// <param name="address">response sender's address</param>
    public void SetData(DiscoveryResponse response, string address)
    {
        if (!registered.ContainsKey(address))
        {
            var content = GetComponent<ScrollRect>().content.transform;
            var button = Instantiate(buttonTemplate, content);
            button.GetComponent<ServerBrowserButtonScript>().SetButtonInfo(response.hostName, address, response.connectedCount);
            registered.Add(address, button.gameObject);
        }
        else
        {
            registered.TryGetValue(address, out GameObject button);
            button.GetComponent<ServerBrowserButtonScript>().UpdatePlayerCount(response.connectedCount);
        }
    }

    /// <summary>
    /// Clears old entries
    /// </summary>
    private void ClearBrowserList()
    {
        var content = GetComponent<ScrollRect>().content.transform;
        if (content.childCount != 0)
        {
            for (int i = content.childCount; i > 0; i--)
            {
                Destroy(content.GetChild(i - 1).gameObject);
            }
        }
    }
}