using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillTriggerManager : NetworkBehaviour
{
    [SerializeField] private GameObject deathEffect;
    private List<GameObject> triggers;
    private Shake shake;

    private void Awake()
    {
        shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>();
        for (int i = 0; i < transform.childCount; i++)
        {
            triggers.Add(transform.GetChild(i).gameObject);
        }
    }

    public void KillPlayer(NetPlayerScript player)
    {
        Instantiate(deathEffect, player.transform);
        if (player.isLocalPlayer)
        {
            StartCoroutine(shake.ShakeCam(0.15f, 0.4f));
        }
    }
}