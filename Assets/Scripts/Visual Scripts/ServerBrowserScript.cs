using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ServerBrowserScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonTemplate;
    private Dictionary<string, GameObject> registered;

    public ServerBrowserButtonScript CurrentSelected;

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject.FindGameObjectWithTag("mainMenuUI").SetActive(true);
                Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        registered = new Dictionary<string, GameObject>();
        ClearBrowserList();
        FindObjectOfType<MyNetworkDiscovery>().StartDiscovery();
    }

    public void StartClient()
    {
        FindObjectOfType<MyNetworkManager>().networkAddress = CurrentSelected.Address;
        FindObjectOfType<MyNetworkManager>().StartClient();
    }

    public void StartHost()
    {
        var manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        if (!manager)
        {
            Debug.LogError("Manager not found");
        }
        else
        {
            if (!NetworkServer.active && !NetworkClient.isConnected)
            {
                var netManager = GameObject.FindGameObjectWithTag("networkManager").GetComponent<MyNetworkManager>();
                netManager.gameObject.GetComponent<MyNetworkDiscovery>().StopDiscovery();
                netManager.HostName = GameObject.FindGameObjectWithTag("hostNameField").GetComponent<TMP_InputField>().text;
                netManager.StartHost();
            }
        }
    }

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
            GameObject button;
            registered.TryGetValue(address, out button);
            button.GetComponent<ServerBrowserButtonScript>().UpdatePlayerCount(response.connectedCount);
        }
    }

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