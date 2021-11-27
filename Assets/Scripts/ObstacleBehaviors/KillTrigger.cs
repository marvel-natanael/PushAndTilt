using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    [SerializeField] private GameObject deathEffect;
    private Shake shake;

    private void Start()
    {
        shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>();
    }

    private void KillPlayer(Transform loc)
    {
        Instantiate(deathEffect, loc);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
<<<<<<< Updated upstream:Assets/Scripts/KillTrigger.cs
            killPlayer(collision.transform);
            StartCoroutine(shake.shakeCam(0.15f, 0.4f));
=======
            KillPlayer(collision.transform);
            StartCoroutine(shake.ShakeCam(0.15f, 0.4f));
            AnimManager.Instance.showDeathAnim();
>>>>>>> Stashed changes:Assets/Scripts/ObstacleBehaviors/KillTrigger.cs
        }
    }
}