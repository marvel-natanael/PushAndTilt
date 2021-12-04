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
    void Start()
    {
        volume.profile.TryGetSettings(out bloom);
    }

    // Update is called once per frame
    void Update()
    {
        if(increment)
        {
            value += 0.1f;
            if (value >= 10.0f) increment = false;
        }
        else
        {
            value -= 0.1f;
            if (value <= 0.5f) increment = true;
        }
        bloom.intensity.value = value;
    }
}
