using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloatFadeScript : MonoBehaviour
{
    private float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime * 1;
        if (timer > 1)
        {
            Destroy(gameObject);
        }
    }
}