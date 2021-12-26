using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ParticleSystem grindEffect;

    private void Start()
    {
        if (!(grindEffect = GetComponentInChildren<ParticleSystem>()))
            Debug.LogError($"{ToString()}: grindEffect not found");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grindEffect.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grindEffect.gameObject.SetActive(false);
        }
    }
}