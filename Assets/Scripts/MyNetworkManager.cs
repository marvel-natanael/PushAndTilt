using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject groundObject;
    private void CreatePlatform()
    {
        var instance = Instantiate(groundObject);
        instance.transform.position = new Vector3(0f, GameScreen.Corner_BottomLeft.y, 0f);
        instance.transform.localScale = new Vector3(2f, 3f, 1f);
    }

    public override void OnStartHost()
    {
        GameScreen.CalculateScreen();
        CreatePlatform();
        base.OnStartHost();
    }

    public override void OnStartClient()
    {
        CreatePlatform();
        base.OnStartClient();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }
}