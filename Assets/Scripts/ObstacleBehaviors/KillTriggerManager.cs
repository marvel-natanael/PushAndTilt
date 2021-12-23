using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillTriggerManager : NetworkBehaviour
{
    private GameManager manager;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Transform deathPoint;
    private Shake shake;

    [Client]
    public void ClientKillPlayer(NetPlayerScript player)
    {
        if (manager.Running)
        {
            Debug.Log($"manager.Running = {manager.Running}");
            Debug.Log($"localPlayer = {player.isLocalPlayer}");
            if (player.isLocalPlayer)
            {
                ClientKill(player);
                player.CmdDie(deathPoint);
            }
        }
    }

    [Client]
    private void ClientKill(NetPlayerScript player)
    {
        FindObjectOfType<EndGameUIScript>().ShowLose(player.PlayerName);
        Instantiate(deathEffect, NetworkClient.connection.identity.transform);
        StartCoroutine(shake.ShakeCam(0.15f, 0.4f));
    }

    private void Awake()
    {
        if (!(manager = FindObjectOfType<GameManager>()))
            Debug.LogError($"{ToString()}: manager not found");
        if (!(shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>()))
            Debug.LogError($"{ToString()}: shake not found");
        if (!(deathPoint = GameObject.FindGameObjectWithTag("deathPoint").transform))
            Debug.LogError($"{ToString()}: deathPoint not found");
    }
}