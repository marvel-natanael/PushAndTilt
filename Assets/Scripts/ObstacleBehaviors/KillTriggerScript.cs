using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTriggerScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.parent.GetComponent<KillTriggerManager>().KillPlayer(collision.gameObject.GetComponent<NetPlayerScript>());
        }
    }
}