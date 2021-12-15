using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerJoinLabelScript : MonoBehaviour
{
    private TextMeshProUGUI nameLabel;
    [HideInInspector] public string Text;

    [SerializeField]
    private float duration;

    private float current;

    private void Awake()
    {
        nameLabel = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        nameLabel.text = Text;
        current = 0f;
    }

    private void Update()
    {
        if (current < duration)
        {
            current += Time.deltaTime;
            var val = (current / duration) * 90f;
            nameLabel.color = new Color(255f, 255f, 255f, Mathf.Cos(val));
        }
        if (current > duration)
        {
            Destroy(gameObject);
        }
    }
}