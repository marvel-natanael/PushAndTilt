using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    private GameManager manager;
    private MyNetworkManager netManager;
    [SerializeField] private LobbyUIScript lobbyUI;
    [SerializeField, SyncVar(hook = nameof(SetReadyCount))] private int readyCount;

    public override void OnStartClient()
    {
        if (!(manager = FindObjectOfType<GameManager>()))
        {
            Debug.LogError($"{ToString()}: manager not found");
        }
        if (!(netManager = FindObjectOfType<MyNetworkManager>()))
        {
            Debug.LogError($"{ToString()}: netManager not found");
        }
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        SetReadyCount(0, 0);
        base.OnStartServer();
    }

    /// <summary>
    /// Hook function to set the number of players ready
    /// </summary>
    /// <remarks>
    /// This function is used when a player is connected, but the game hasn't started yet.
    /// </remarks>
    /// <param name="_old">[unused]</param>
    /// <param name="_new">new number</param>
    private void SetReadyCount(int _old, int _new)
    {
        readyCount = _new;
        Debug.Log($"{ToString()}: readycount is now {_new}");
    }

    [Server]
    private void ServerSetReadyCount(bool state)
    {
        if (state) SetReadyCount(readyCount, readyCount + 1);
        else SetReadyCount(readyCount, readyCount - 1);
    }

    [Command]
    public void CmdSetPlayerReadyState(bool state)
    {
        RpcSetPlayerReadyState(state);
    }

    [TargetRpc]
    private void RpcSetPlayerReadyState(bool state)
    {
    }
}