using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    #region Fields

    private GameManager manager;
    private MyNetworkManager netManager;
    [SerializeField] private LobbyUIScript lobbyUI;

    //Ready fields

    [SerializeField, SyncVar] private int readyCount;
    private bool localReady;

    //CountDown

    [SerializeField] private float startingTime;
    private bool isCounting;

    #endregion Fields

    #region Properties

    public bool LocalReady => localReady;

    #endregion Properties

    #region Server_Functions

    [Server]
    private void ServerStartGame()
    {
        manager.ServerSetRunningState(true);
        Debug.Log($"{ToString()}: ServerStartGame() done....");
    }

    [Server]
    private void ServerStartCountDown(float time)
    {
        StartCoroutine(CountDown(time));
        Invoke(nameof(ServerStartGame), time);
        Debug.Log($"{ToString()}: ServerStartCountDown({time}) done...");
    }

    [Server]
    private void ServerAbortCountDown()
    {
        StopCoroutine(nameof(CountDown));
        CancelInvoke(nameof(ServerStartGame));
        Debug.Log($"{ToString()}: ServerAbortCountDown() done...");
    }

    /// <summary>
    /// Server-side function to set server's <c>readyCount</c> count
    /// </summary>
    /// <param name="state">New value</param>
    [Server]
    private void ServerSetReadyCount(bool state)
    {
        if (state)
        {
            if (readyCount < 5)
            {
                readyCount++;
            }
            else Debug.LogWarning($"{ToString()}: readyCount is out of range, something is not right...");
        }
        else
        {
            if (readyCount > 0)
            {
                readyCount--;
            }
            else Debug.LogWarning($"{ToString()}: readyCount is out of range, something is not right...");
        }

        //ReadyCounter
        var connCount = NetworkServer.connections.Count;
        if (connCount > 1)
        {
            if (readyCount == connCount)
            {
                if (!isCounting)
                {
                    Debug.Log($"{ToString()}: ServerSetReadyCount({state}): everyone is ready");
                    ServerStartCountDown(startingTime);
                    RpcStartCountdown(startingTime);
                    isCounting = true;
                }
            }
            else
            {
                if (isCounting)
                {
                    Debug.Log($"{ToString()}: ServerSetReadyCount({state}): game is aborted");
                    ServerAbortCountDown();
                    RpcAbortCountDown();
                    isCounting = false;
                }
            }
        }
    }

    #endregion Server_Functions

    #region Client_Functions

    [Client]
    private void ClientStartCountDown(float time)
    {
        StartCoroutine(CountDown(time));
        Debug.Log($"{ToString()}: ClientStartCountDown({time})");
    }

    [Client]
    private void ClientAbortCountDown()
    {
        StopCoroutine(nameof(CountDown));
        lobbyUI.CW_numEmpty();
        Debug.Log($"{ToString()}: ClientAbortCountDown()");
    }

    #endregion Client_Functions

    #region Commands

    /// <summary>
    /// Command to set this connection's ready state
    /// </summary>
    /// <remarks>Only to be called by a client</remarks>
    /// <param name="state">New ready state</param>
    /// <param name="conn">Leave this blank to retrieve this connection</param>
    [Command(requiresAuthority = false)]
    public void CmdSetPlayerReadyState(bool state, NetworkConnectionToClient conn = null)
    {
        ServerSetReadyCount(state);
        conn.identity.GetComponent<NetPlayerScript>().ServerSetPlayerReadyState(state);
        RpcSetPlayerReadyState(conn, state);
        Debug.Log($"{ToString()}: Cmd thrown, readyState changed...");
    }

    #endregion Commands

    #region ClientRPCs

    /// <summary>
    /// Rpc to verify server's ready state
    /// </summary>
    /// <remarks>Only to be called by the server</remarks>
    /// <param name="conn">this connection</param>
    /// <param name="state">new state</param>
    [TargetRpc]
    private void RpcSetPlayerReadyState(NetworkConnection conn, bool state)
    {
        localReady = state;
        Debug.Log($"{ToString()}: RPC fetch, readyState changed...");
    }

    [ClientRpc]
    private void RpcStartCountdown(float time)
    {
        ClientStartCountDown(time);
    }

    [ClientRpc]
    private void RpcAbortCountDown()
    {
        ClientAbortCountDown();
    }

    #endregion ClientRPCs

    public override void OnStartClient()
    {
        localReady = false;
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        readyCount = 0;
        localReady = false;
        base.OnStartServer();
    }

    private void Start()
    {
        if (!(manager = FindObjectOfType<GameManager>()))
            Debug.LogError($"{ToString()}: manager not found");
    }

    private IEnumerator CountDown(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            lobbyUI.CW_numUpdate(time);
            yield return null;
        }
    }
}