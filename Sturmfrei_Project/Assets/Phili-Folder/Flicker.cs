using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    private Light spotLight;
    public float maxIntensity = 6f;
    public float minIntensity = 1f;
    public float pulseSpeed = 20f; //here, a value of 0.5f would take 2 seconds and a value of 2f would take half a second
    public float targetIntensity = 6f;
    public float currentIntensity;

    void Start()
    {
        spotLight = GetComponent<Light>();
    }
    void Update()
    {
        currentIntensity = Mathf.MoveTowards(spotLight.intensity, targetIntensity, Time.deltaTime * pulseSpeed);
        if (currentIntensity >= maxIntensity)
        {
            currentIntensity = maxIntensity;
            targetIntensity = minIntensity;
        }
        else if (currentIntensity <= minIntensity)
        {
            currentIntensity = minIntensity;
            targetIntensity = maxIntensity;
        }
        spotLight.intensity = currentIntensity;
    }
}
