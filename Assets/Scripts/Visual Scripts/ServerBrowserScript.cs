using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonTemplate;

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
        ClearBrowserList();
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
            manager.SetHostName("", GameObject.FindGameObjectWithTag("hostNameField").GetComponent<TMP_InputField>().text);
            if (!NetworkServer.active && !NetworkClient.isConnected)
            {
                var netManager = GameObject.FindGameObjectWithTag("networkManager");
                netManager.GetComponent<MyNetworkDiscovery>().StopDiscovery();
                netManager.GetComponent<MyNetworkManager>().StartHost();
            }
        }
    }

    public void SetData(DiscoveryResponse response, string address)
    {
        var content = GetComponent<ScrollRect>().content.transform;
        var button = Instantiate(buttonTemplate, content);
        button.GetComponent<ServerBrowserButtonScript>().SetButtonInfo(response.HostName, address, response.ConnectedCount);
    }

    private void ClearBrowserList()
    {
        var content = GetComponent<ScrollRect>().content.transform;
        while (content.childCount != 0)
        {
            Destroy(content.GetChild(0).gameObject);
        }
    }

    public void RefreshBrowserList()
    {
        ClearBrowserList();
        FindObjectOfType<MyNetworkDiscovery>().StartDiscovery();
    }
}