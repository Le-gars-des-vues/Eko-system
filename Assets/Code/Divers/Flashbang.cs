using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashbang : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] float maxInterval = 0.5f;
    [SerializeField] float maxFlicker = 0.2f;

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
        timer += Time.deltaTime;
        if (timer > delay)
        {
            ToggleLight();
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
            light2D.intensity = Random.Range(0.6f, defaultIntensity);
            delay = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }
}
