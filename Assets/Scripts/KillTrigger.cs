using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    public GameObject deathEffect;
    private Shake shake;

    private void Start()
    {
        shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>();
    }
    void killPlayer(Transform loc)
    {
        Instantiate(deathEffect, loc);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            killPlayer(collision.transform);
            StartCoroutine(shake.shakeCam(0.15f, 0.4f));
        }
    }
}
