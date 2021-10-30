using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimManager : MonoBehaviour
{
    private static AnimManager _instance;
    public GameObject backButton;
    public static AnimManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AnimManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        backButton.gameObject.SetActive(false);
    }
    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public IEnumerator animFadeIn(CanvasGroup image)
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            image.alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }
    public IEnumerator animFadeOut(CanvasGroup image)
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            image.alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
        backButton.gameObject.SetActive(true);
    }
    public IEnumerator shake(GameObject obj, float duration, float magnitude)
    {
        Vector3 orignalPosition = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            obj.transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        obj.transform.position = orignalPosition;
    }
}
