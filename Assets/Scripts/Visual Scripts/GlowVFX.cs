using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlowVFX : MonoBehaviour
{
    public PostProcessVolume volume;
    Bloom bloom;
    bool increment = true;
    [SerializeField]
    float value = 0.5f;
    [SerializeField]
    float maxvalue = 50.0f;
    [SerializeField]
    float minvalue = 0.0f;
    void Awake()
    {
        volume.profile.TryGetSettings(out bloom);
    }

    // Update is called once per frame
    void Update()
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
