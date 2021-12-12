using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyUIPrefab;
    [SerializeField] private LobbyUIScript lobbyUI;
    [SerializeField, SyncVar(hook = nameof(SetReadyCount))] private int readyCount;
    private GameManager manager;
    private MyNetworkManager netManager;

    private void Awake()
    {
        lobbyUI = Instantiate(lobbyUIPrefab).GetComponent<LobbyUIScript>();
        manager = FindObjectOfType<GameManager>();
        netManager = FindObjectOfType<MyNetworkManager>();
        SetReadyCount(0, 0);
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
        Debug.Log("Hook fired: SetReadyCount() on LobbyManager.cs with new value of " + _new);
    }

    public void ToggleReady()
    {
        var localPlayerScript = NetworkClient.localPlayer.gameObject.GetComponent<NetPlayerScript>();
        if (localPlayerScript.isReady)
        {
            SetReadyCount(readyCount, readyCount + 1);
        }
        else
        {
            SetReadyCount(readyCount, readyCount - 1);
        }
    }
}