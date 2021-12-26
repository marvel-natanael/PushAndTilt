using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlowVFX : MonoBehaviour
{
    public PostProcessVolume volume;
    private Bloom bloom;
    private bool increment = true;

    [SerializeField]
    private float value = 0.5f;

    [SerializeField]
    private float maxvalue = 50.0f;

    [SerializeField]
    private float minvalue = 0.0f;

    private void Awake()
    {
        volume.profile.TryGetSettings(out bloom);
    }

    // Update is called once per frame
    private void Update()
    {
        if (increment)
        {
            value += 0.1f;
            if (value >= maxvalue) increment = false;
        }
        else
        {
            value -= 0.1f;
            if (value <= minvalue) increment = true;
        }
        bloom.intensity.value = value;
    }
}