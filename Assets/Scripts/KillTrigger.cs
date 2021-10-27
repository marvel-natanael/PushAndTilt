using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillTrigger : MonoBehaviour
{
    public GameObject deathEffect;
    private AnimManager animManager;
    public CanvasGroup loseAnimImage;
    private void Start()
    {
        animManager = GameObject.Find("AnimManager").GetComponent<AnimManager>();
        loseAnimImage.alpha = 0f;
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
            StartCoroutine(animManager.shake(GameObject.FindGameObjectWithTag("MainCamera"),0.15f, 0.4f));
            StartCoroutine("loseAnim");
        }
    }
    IEnumerator loseAnim()
    {
        StartCoroutine(animManager.animFadeIn(loseAnimImage));
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(animManager.animFadeOut(loseAnimImage));
    }
}

