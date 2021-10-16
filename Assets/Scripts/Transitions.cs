using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Transitions : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void transitionTo()
    {
        StartCoroutine(loadTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator loadTransition(int index)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(index);
    }
}
