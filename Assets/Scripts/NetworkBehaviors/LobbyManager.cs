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
    [SerializeField] private bool enableGUI;

    //Ready fields

    [SerializeField, SyncVar] private int readyCount;
    private bool localReady;

    //CountDown

    private float startingTime;
    private bool isCounting;

    #endregion Fields

    #region Properties

    public bool LocalReady => localReady;

    #endregion Properties

    #region Server_Functions

    /// <summary>
    /// Server-side function to set server's <c>readyCount</c> count
    /// </summary>
    /// <param name="state">New value</param>
    [Server]
    public void ServerSetReadyCount(bool state)
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
                    RpcStartCountdown(startingTime);
                    ServerStartCountDown(startingTime);
                    isCounting = true;
                }
            }
            else
            {
                if (isCounting)
                {
                    RpcAbortCountDown();
                    ServerAbortCountDown();
                    isCounting = false;
                }
            }
        }
    }

    [Server]
    private void ServerStartCountDown(float time)
    {
        Invoke(nameof(ServerStartGame), time);
    }

    [Server]
    private void ServerAbortCountDown()
    {
        CancelInvoke(nameof(ServerStartGame));
    }

    [Server]
    private void ServerStartGame()
    {
        manager.ServerStartGame();
        manager.ServerSetAlivePlayerCount(NetworkServer.connections.Count);
        RpcDestroyLobbyUI();
        ServerMakeEveryoneAlive();
    }

    [Server]
    private void ServerMakeEveryoneAlive()
    {
        foreach (var conn in NetworkServer.connections)
        {
            conn.Value.identity.GetComponent<NetPlayerScript>().ServerSetPlayerAliveState(true);
        }
    }

    #endregion Server_Functions

    #region Client_Functions

    [Client]
    private void ClientStartCountDown(float time)
    {
        StartCoroutine(CountDown(time));
    }

    [Client]
    private void ClientAbortCountDown()
    {
        StopAllCoroutines();
        lobbyUI.CW_numEmpty();
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
        if (NetworkServer.connections.Count > 1)
        {
            ServerSetReadyCount(state);
            conn.identity.GetComponent<NetPlayerScript>().ServerSetPlayerReadyState(state);
            RpcSetPlayerReadyState(conn, state, true);
        }
        else
        {
            RpcSetPlayerReadyState(conn, false, false);
        }
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
    private void RpcSetPlayerReadyState(NetworkConnection conn, bool state, bool success)
    {
        if (success)
        {
            localReady = state;
        }
        else
        {
            lobbyUI.UI_ShowError($"There should be more than one player to be ready");
        }
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

    [ClientRpc]
    private void RpcDestroyLobbyUI()
    {
        Destroy(lobbyUI.gameObject);
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
        startingTime = 5f;
        base.OnStartServer();
    }

    private void Start()
    {
        if (!(manager = FindObjectOfType<GameManager>()))
            Debug.LogError($"{ToString()}: manager not found");
        if (!(lobbyUI = FindObjectOfType<LobbyUIScript>()))
            Debug.LogError($"{ToString()}: lobbyUI not found");
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