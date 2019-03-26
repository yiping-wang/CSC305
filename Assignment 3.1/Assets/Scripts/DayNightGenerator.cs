using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightGenerator : MonoBehaviour {
    public Light sun;
    public float lengthOfFulldayInSec = 24f;
    [Range(0, 1)]
    public float currentTimeOfDay;
    public float timeOfDawn;
    public float timeOfSunset;

    float sunInitialIntensity;
    
    void Start() {
        sunInitialIntensity = sun.intensity;
    }
    
    void Update() {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        if (currentTimeOfDay <= timeOfDawn || currentTimeOfDay >= timeOfSunset)
        {
            sun.intensity = 0;
        }
        else
        {
            sun.intensity = sunInitialIntensity;
        }

        currentTimeOfDay += Time.deltaTime / lengthOfFulldayInSec;
        if (currentTimeOfDay >= 1) {
            currentTimeOfDay = 0;
        }
    }
}
