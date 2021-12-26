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
        if (manager.PlayerCount > 2)
            FindObjectOfType<EndGameUIScript>().ShowLose(player.PlayerName);
        Instantiate(deathEffect, NetworkClient.connection.identity.transform);
        StartCoroutine(shake.ShakeCam(0.15f, 0.4f));
    }

    private void Start()
    {
        if (!(manager = FindObjectOfType<GameManager>()))
            Debug.LogError($"{ToString()}: manager not found");
        if (!(shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>()))
            Debug.LogError($"{ToString()}: shake not found");
        if (!(deathPoint = GameObject.FindGameObjectWithTag("deathPoint").transform))
            Debug.LogError($"{ToString()}: deathPoint not found");
    }
}