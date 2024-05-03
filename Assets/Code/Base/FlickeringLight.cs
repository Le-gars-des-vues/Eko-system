using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] float maxInterval = 0.5f;
    [SerializeField] float maxFlicker = 0.2f;
    [SerializeField] float minIntensity = 0.6f;
    [SerializeField] bool isFlickering;

    float defaultIntensity;
    bool isOn;
    float timer;
    float delay;

    private void OnEnable()
    {
        light2D = GetComponentInChildren<Light2D>();
        defaultIntensity = light2D.intensity;
    }

    void Update()
    {
        if (isFlickering)
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                ToggleLight();
            }
        }
    }

    void ToggleLight()
    {
        isOn = !isOn;

        if (isOn)
        {
            light2D.intensity = defaultIntensity;
            delay = Random.Range(0, maxInterval);
        }
        else
        {
            light2D.intensity = Random.Range(minIntensity, defaultIntensity);
            delay = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }
}
