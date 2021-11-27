using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameManager manager;
    [SerializeField] private string hostName;

    public string HostName { get => hostName; set => hostName = value; }

    public override void OnStartHost()
    {
        base.OnStartHost();
        var manager = FindObjectOfType<GameManager>();
        manager.SetPlayerCount(manager.PlayerCount, (short)manager.PlayerCount + 1);
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