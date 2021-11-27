using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;

    public void transitionTo()
    {
        StartCoroutine(loadTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator loadTransition(int index)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(index);
    }
}