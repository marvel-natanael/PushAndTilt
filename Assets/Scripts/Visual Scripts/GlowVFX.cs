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
<<<<<<< Updated upstream
    void Start()
=======
    [SerializeField]
    float maxvalue = 50.0f;
    [SerializeField]
    float minvalue = 0.0f;
    void Awake()
>>>>>>> Stashed changes
    {
        volume.profile.TryGetSettings(out bloom);
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if(increment)
        {
            value += 0.1f;
            if (value >= 10.0f) increment = false;
=======
        if (increment)
        {
            value += 0.1f;
            if (value >= maxvalue) increment = false;
>>>>>>> Stashed changes
        }
        else
        {
            value -= 0.1f;
<<<<<<< Updated upstream
            if (value <= 0.5f) increment = true;
=======
            if (value <= minvalue) increment = true;
>>>>>>> Stashed changes
        }
        bloom.intensity.value = value;
    }
}
