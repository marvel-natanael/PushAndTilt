using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject groundObject;
    public override void OnStartHost()
    {
        var instance = Instantiate(groundObject);
        instance.transform.position = new Vector3(0f, GameManager.ScreenPointZero.y, 0f);
        instance.transform.localScale = new Vector3(2f, 3f, 1f);
        base.OnStartHost();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {

        base.OnClientConnect(conn);
    }
}
