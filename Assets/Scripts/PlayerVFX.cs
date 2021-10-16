using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem GrindEffect;
    void Start()
    {
        GrindEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GrindEffect.gameObject.SetActive(true);
            Debug.Log("fdfs");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GrindEffect.gameObject.SetActive(false);
            Debug.Log("fdfd");
        }
    }

}
