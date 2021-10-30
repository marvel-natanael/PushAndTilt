using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private AnimManager animManager;
    public CanvasGroup winAnimImage;
    private void Start()
    {
        animManager = GameObject.Find("AnimManager").GetComponent<AnimManager>();
        winAnimImage.alpha = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(animManager.shake(GameObject.FindGameObjectWithTag("MainCamera"), 0.15f, 0.4f));
            StartCoroutine("winAnim");
        }
    }
    IEnumerator winAnim()
    {
        StartCoroutine(animManager.animFadeIn(winAnimImage));
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(animManager.animFadeOut(winAnimImage));
    }
}
