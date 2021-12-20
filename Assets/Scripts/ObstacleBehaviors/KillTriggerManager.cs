using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillTriggerManager : NetworkBehaviour
{
    [SerializeField] private GameObject deathEffect;
    private Shake shake;

    private void Awake()
    {
        if (!(shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>()))
            Debug.LogError($"{ToString()}: shake not found");
    }

    [Server]
    public void KillPlayer(NetPlayerScript player)
    {
        RpcKill(player.netIdentity.connectionToClient);
        player.CmdDie();
    }

    [TargetRpc]
    private void RpcKill(NetworkConnection conn)
    {
        ClientKill();
    }

    [Client]
    private void ClientKill()
    {
        Instantiate(deathEffect, NetworkClient.connection.identity.transform);
        StartCoroutine(shake.ShakeCam(0.15f, 0.4f));
    }
}