using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameManager manager;
    private string hostName;

    public override void Start()
    {
        base.Start();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        GetComponent<MyNetworkDiscovery>().AdvertiseServer();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (!manager)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        }
        else
        {
            if (manager.PlayersConnected == 5)
            {
                conn.Disconnect();
            }
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //todo: Send player name
    }
}